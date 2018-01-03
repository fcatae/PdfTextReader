# Pipeline #

Execution pipeline to run listeners and block processors.

- Run the listeners
- Run the blocks

Pipeline:

    Pipeline.Input("input")
            .Page(1)  // .AllPages()
            .ParsePdf( PreProcessTables )
            .ParsePdf( ProcessPdfText )
            .ParseBlock( RemoveTableInlineText , Color: green)
            .ParseBlock( FindPageColumns )
            .ParseBlock( BreakColumns , Color: Orange)
                .Show( "error1", Color: orange )
                .Show( "error2", Color: gray )
            .ParseBlock( RemoveHeader , Color: blue)
            .ParseBlock( RemoveFooter , Color: blue)
            .Show( Color: yellow )

            .ParseBlock( CreateLines )
            .CreateText( CreateStructures )

            