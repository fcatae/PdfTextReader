# Pipeline #

Execution pipeline to run listeners and block processors.

- Run the listeners
- Run the blocks

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


            