Architecture
=============

1. PDF Parser
2. Semantic Tagging
3. Linker

Current Status
===============

Parser PDF is quite robust and it gathers text from the document.
The next step is to understand the text to extract the metadata,
and feed the Linker.

Work in progress:
- Identify the article (title, body, signature)
- Working on the Linker



# Block Hierarchy #

BlockPage -> BlockColumn -> BlockLine -> Block


# Pipeline #

Execution pipeline to run listeners and block processors.

- Run the listeners
- Run the blocks

Stream:

    .Output<TextLine>()
        .Tool<CreateStructure>()
    .Output<TextParagraphs>()
        .Tool<CreateParagraphs>()
    .Output<TextStructure>()

                            // show text lines
                            .Process( PrintAnalytics.ShowTextLine($"bin/{page}-out-txtline.xml") )
                            .Process2(new PrintAnalytics2.ShowLines($"bin/{page}-out-txtline2.xml"))

                            // TextLine -> TextStructure
                            .ConvertText<CreateParagraphs, TextStructure>()  
                                .Filter<Remover>()
                                .Show()

                                .Log("bin/{page}-step-1")
                                .Filter<Remover>()
                                .Log("bin/{page}-step-2")
                                .Run( t => t.Calculate<Stat>()
                                            .Log("bin/{page}-step-2-stat"))
                                .Stat<FontStats>("bin/{page}-step-2-stat")

.Log("bin/{page}-step-3", p => p)
                                .Run(
                                    a => {},
                                    b => {},
                                    c => {}
                                )

                                .Process( s => new DebugLog("", s) )

                                LogXml/LogJson/Log<T>ToString
                                Text(Serialize)
                                Block(Show) => true/false
                                Save/Load

                                ApplyHtml<Title>( t => t.Center() )

                            // show text structures
                            .Process( PrintAnalytics.ShowTextStructure($"bin/{page}-out-txtstr.xml") )
                            .Process2(new PrintAnalytics2.ShowStructures($"bin/{page}-out-txtstr2.xml"))



Pipeline:

    Pipeline.Input("input") //.Output
            .Page(1)  // .AllPages( p => p.CurrentPage )
            .ParsePdf( PreProcessTables )             
            .StoreResult( "INLINETABLES" )
                .Output("table-output")
                .Show( b => b.op ==1, Color: green )
            .Output("lines")
            .ParsePdf( ProcessPdfText )
            .ParseBlock( RemoveTableInlineText("INLINETABLES"))
            .ParseBlock( FindPageColumns )
            .ParseBlock( BreakColumns )
                .Validate( BreakColumns, Color: red )
            .ParseBlock( RemoveHeader , Color: blue)
            .ParseBlock( RemoveFooter , Color: blue)
                .Validate( ValidFooter, p => PipelineException() )
            .ParseBlock( CreateLines )
            .ParseBlock( CreateStructures )
                .Show( Color: yellow )
            .ParseText( ProcessParagraphs )
            .ParseText( ProcessStructure )
                .Output("structures")
                .Show( Color: red )
            .ParseContent( Articles )
                .SaveXml( p => $"file-{p.page}")


            
# Old Architecture

# 1. Rewriting the parse module #

`UserWriter` contains major codebase separated into 4 different categories.

### MainLogic ###

Process
- ActiveTables
- ProcessBlockExtra
- ProcessBlock
- FinalProcess

### Blocklist ###
  
Structure
- TryMergeBlockSets
- RemoveList

Region
- FindHeader
- FindFooter

Visual
- DrawRectangle (depends on PDFCanvas)
- PrintText

Complex code
- BreakBlockSets
- CalculateCenterBreak

### BlockSet ###

Calculation
- GetBlockWithLargerWidth
- GetBlockWithSmallerWidth
- HasAreaOverlap
- HasOverlap(segA, segB)
- HasOverlap(area,x,y)

Table
- IsInTable

### StructureItem ###

- ProcessStructure
- HighlightStructureItems

            
- Problemas com tabela
- Problemas com imagens (dz67, dz68)
Assumir:

- Capa
- Texto

- Figuras
- Tabelas
- 1, 2 ou 3 colunas
- Todos documentos possuem 3 colunas

Materia:
- Titulo
- Ementa
- Corpo
- Assinatura

Texto
- Alinhamento central
- Alinhamento justificado
- Tabulacao esquerda: Primeira linha do paragrafo
- Tabulacao direita: Assinatura 

Titulo:
- Letra maiúscula

Assinatura: 
- Letra maiuscula

Artigos
- Ordenados

Alinhamento:
- Central
- Justificado esquerda
- Justificado esquerda com tabulacao
- Justificado esquerda com margem grande (ementa)
- Justificado direita
- Justificado direito com tabulacao
- Nenhum
- Central em relação a outro box
- Dentro de tabelas

Apenas textos justificados direita podem ter variacao de espaçamento.

Cabeçalho padrão
Rodapé padrão

# Quebrar blockset longos

Implementacao usando o centro do overlap

dz002: decreto            