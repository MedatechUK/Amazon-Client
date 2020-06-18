' Stan edit this!

Imports priority

Public Class ldef_Order : Inherits priority.Loading

    Sub New()
        With Me

            .Clear()
            .Table = "ZSPU_WEBORDLOAD"
            .Procedure = "ZSPU_WEBORDLOAD"


            .AddColumn(1) = New LoadColumn("WEBORD", tColumnType.typeCHAR, 8)
            .AddColumn(1) = New LoadColumn("STATUS", tColumnType.typeCHAR, 12)
            .AddColumn(1) = New LoadColumn("CURDATE", tColumnType.typeDATE, 14)

            .AddColumn(2) = New LoadColumn("PARTNAME", tColumnType.typeCHAR, 22)
            .AddColumn(2) = New LoadColumn("QUANT", tColumnType.typeINT, 13)
            .AddColumn(2) = New LoadColumn("PRICE", tColumnType.typeREAL, 9)
            .AddColumn(2) = New LoadColumn("DUEDATE", tColumnType.typeDATE, 8)

            .AddColumn(3) = New LoadColumn("CUSTDES", tColumnType.typeCHAR, 48)
            .AddColumn(3) = New LoadColumn("PHONENUM", tColumnType.typeCHAR, 20)
            .AddColumn(3) = New LoadColumn("EMAIL", tColumnType.typeCHAR, 72)
            .AddColumn(3) = New LoadColumn("ADDRESS", tColumnType.typeCHAR, 80)
            .AddColumn(3) = New LoadColumn("ADDRESS2", tColumnType.typeCHAR, 80)
            .AddColumn(3) = New LoadColumn("ADDRESS3", tColumnType.typeCHAR, 80)
            .AddColumn(3) = New LoadColumn("STATE", tColumnType.typeCHAR, 40)
            .AddColumn(3) = New LoadColumn("ZIP", tColumnType.typeCHAR, 10)

        End With
    End Sub

End Class
