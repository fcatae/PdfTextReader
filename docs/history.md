# Pipeline #

## Week 3: Rearchitecture with Pipeline ##

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


            
## Week 2: Use iText library to extract valuable information from PDF ##

`UserWriter` contains major codebase separated into 4 different categories.
- MainLogic
- BlockLine
- Block
- Structure

Problems found            
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