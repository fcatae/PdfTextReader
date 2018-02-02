PDFTextReader
===============

Tool to extract data from Diario Oficial da União (DOU) stored in PDF file.

# Architecture #
 
The project has a pipeline for processing the incoming PDF file.

## Problem: PDF has no concept of column, paragraph or line ##

PDF is just a collection of characters spread out in a document page. Sometimes we can tell that characters form a word due to its spacing characteristics. However, this spacing can be variable and breaks the word.

![blocks](docs/images/01blocks.png)

Examples:
- Por-t-aria (*correct: Portaria*)
- A-T-O DECLARA-TÓRIO (*correct: ATO DECLARATÓRIO*)
- CONSUL-T-A (*correct: CONSULTA*)

This block behavior brought us interesting surprises:

- Hidden characters to human (font size too small), but visible to machines
- Characters composed of multiple characters. Eg, Euro sign (€) is represented not as a Unicode, but with overlapped characters: `C` and `=`
- Characters in different sizes. Eg, ordinal number (Nº) is represented with `N`, `o`, `-` and it is read as `No-` by the machine. However, the `o` and `-` are printed with smaller fonts and higher baselines.


## Defining the Lines ##

The first problem was to find out when the characters are grouped into a single line. It is usually easy to tell that the characters are on the same baseline.

![lines](docs/images/02lines.png)

Major exceptions:
- Table rows (large spacing between the columns)
- Overlapped characters to represent a special character
- Multiple font sizes and types (eg, words in either bold and italic in the line - you cannot say that the line is all bold and italic)

## Identifying the Block Sets ##

The next step is to group the lines into block sets. We want to measure the distance between the lines, and have the closest lines forming a blockset. At least one blockset per column, but no blockset spans multiple columns.

![blocksets](docs/images/03blocksets.png)

The image shows that sometimes we incorrectly classify the blocks. 

One of the most difficult task was to identify the Header and Footer of the page. There are multiple variations of header/footer in size and sometimes they are not present in the document. On the other hand, it is easy to check whether we correctly classified header/footer along the whole document.

Thus, analyzing headers and footers in the scope of a page is difficult. However, it is easy to analyze in the global scope of the document.


## PDF Printing Order ##

A file is a sequence of bytes that represents the characters and words of a PDF file. However, there is no high-level concept of lines, paragraphs or columns. We can say that PDF behaves more like a PostScript instruction set rather than a semantic HTML document. 

![followLine](docs/images/04followline.png)

The characters and words are presented in no specific order. In our case study, we usually see the header and footer in the beginning of the file. But we also found exceptions to this.

The challenge is to identify what is a title or content (body). There is no explicit mark or tag to differentiate the content type. So we usually tell the title by the font size, the visual centered aligment and the upper case. Again, that is not a strict rule though.

There is no concept of tables or forms in PDF. Compared to HTML, there is no `<table><tr><td>` neither `<form><input type=..>`. We have only characters printed in front of a background image, which contains the grid lines.

![tables](docs/images/05tables.png)

In order to reconstruct the tables, we have to parse all the line segments and glue them together. Then we have the concept of what is a rectangle, and then we look for text around and finally we have a table.


## Blockset Order ##

Once we understand what are text, image and tables, we can lay the blocks in the page. The last problem is to define the correct reading order - that is not simply left-right, up-down.

![order](docs/images/06ordering.png)

The problem is complex because a single page can have 1, 2 or 3 columns. Sometimes the columns can be merged, so it is not unusual to see a column fitting into a 2/3 of the page.


## Pipeline: PDF Parser ##

This initial process consists of a pipeline with ~30 stages. 

```csharp
pipeline.Input($"{inputfolder}/{basename}.pdf")
    .Output($"{outputfolder}/{basename}/parser-output.pdf")
    .AllPages<CreateTextLines>(page =>
                page.ParsePdf<PreProcessTables>()
                    .ParseBlock<IdentifyTables>()             // 1
                .ParsePdf<PreProcessImages>()
                    .ParseBlock<BasicFirstPageStats>()        // 2
                    .ParseBlock<RemoveOverlapedImages>()      // 3
                .ParsePdf<ProcessPdfText>()                   // 4
                    .ParseBlock<RemoveSmallFonts>()           // 5
                    .ParseBlock<MergeTableText>()             // 6
                    .ParseBlock<HighlightTextTable>()         // 7
                    .ParseBlock<RemoveTableText>()            // 8
                    .ParseBlock<ReplaceCharacters>()          // 9
                    .ParseBlock<GroupLines>()                 // 10
                    .ParseBlock<RemoveTableDotChar>()         // 11
                        .Show(Color.Yellow)
                        .Validate<RemoveHeaderImage>().ShowErrors(p => p.Show(Color.Purple))
                    .ParseBlock<RemoveHeaderImage>()          // 12
                    .ParseBlock<FindInitialBlocksetWithRewind>()  // 13
                        .Show(Color.Gray)
                    .ParseBlock<BreakColumnsLight>()          // 14
                    .ParseBlock<AddTableSpace>()              // 15
                    .ParseBlock<RemoveTableOverImage>()       // 16
                    .ParseBlock<RemoveImageTexts>()           // 17
                    .ParseBlock<AddImageSpace>()              // 18
                        .Validate<RemoveFooter>().ShowErrors(p => p.Show(Color.Purple))
                    .ParseBlock<RemoveFooter>()               // 19
                    .ParseBlock<AddTableHorizontalLines>()    // 20
                    .ParseBlock<RemoveBackgroundNonText>()    // 21
                        .ParseBlock<BreakColumnsRewrite>()    // 22

                    .ParseBlock<BreakInlineElements>()        // 23
                    .ParseBlock<ResizeBlocksets>()            // 24
                    .ParseBlock<ResizeBlocksetMagins>()       // 25
                    .ParseBlock<OrderBlocksets>()           // 26

                    .ParseBlock<OrganizePageLayout>()         // 27
                    .ParseBlock<MergeSequentialLayout>()      // 28
                    .ParseBlock<ResizeSequentialLayout>()     // 29
                        .Show(Color.Orange)
                        .ShowLine(Color.Black)

                    .ParseBlock<CheckOverlap>()               // 30

                        .Validate<CheckOverlap>().ShowErrors(p => p.Show(Color.Red))
                        .Validate<ValidatePositiveCoordinates>().ShowErrors(p => p.Show(Color.Red))
                    .PrintWarnings()
```

The current process is quite accurate (error: 1-2%). Most of the errors are caused by inline table. In order to improve this, we have to review our current process: 

1) Extract text
2) Identify the columns.

Thus, we have to first identify the blocksets and fit into columns. Then we extract the text and parse the tables.

1) Identify the columns.
2) Extract text

It looks similar - but it requires us to scan the PDF files twice.


## Defining the Text Alignment ##

The goal of the PDF Parser Pipeline is to generate a list of ordered blocksets that we can use to extract the text. The text itself has margins relative to the column (not to the page), which allow us to define the text alignment property.

In our case, we highlight the `centered text in red`, and `right-aligned or floating text in blue`.

![text-align](docs/images/10align.png)


## Edge Cases are Hard ##

This process has also flaws:

1. Large vertical distance between the lines cause us to consider multiple paragraphs rather than a single one.

    ![large vertical distance](docs/images/11linedist.png)


2. Titles and body text with similar vertical distance

    ![vertical distance](docs/images/12paragraph.png)


3. Justified text that are visually centered in the column

    ![justified and centered](docs/images/13centered.png)


4. Centered text that are slightly off to the right, and could be considered as floating to the right

    ![centered off right](docs/images/14notcentered.png)


## Pipeline for Text Processing ##

Classifying the line into Centered, Justified or Floating-Right is quite straightforward. If the margins are equal, then the text is centered. If the margin right is smaller than the left, then it is floating left. Otherwise, it is justified.

However, we noticed several edge cases to be handled. The solution goes through the text to identify paragraph similarities to group them, and specific characteristics to break them apart. 

This process is composed of 7 stages.

```csharp
var articles = ConvertPdfIntoText(basename)
                .ConvertText<CreateTextLineIndex,TextLine>()        // 1
                .ConvertText<PreCreateStructures, TextLine2>()      // 2
                .ConvertText<CreateStructures2, TextStructure>()    // 3
                .ConvertText<PreCreateTextSegments, TextStructureAgg>() // 4
                .ConvertText<AggregateStructures, TextStructure>() // 5
                    .ShowPdf<ShowStructureCentral>($"{basename}-show-central.pdf")
                .ConvertText<CreateTextSegments, TextSegment>()     // 6
                .ConvertText<CreateTreeSegments, TextSegment>()     // 7
                .ToList();
``` 

