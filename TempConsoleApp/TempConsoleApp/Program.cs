// See https://aka.ms/new-console-template for more information
using System.Text.Json;
using TempConsoleApp;

internal class Program
{
    private static void Main()
    {
        Console.WriteLine("Hello, World!");

        CellIdValueField cellIdValueField = new(2);

        string value = cellIdValueField[1];

        cellIdValueField[1] = value+1;

        for(int i = 0; i < cellIdValueField.Size; i++)
        {
            Console.WriteLine(JsonSerializer.Serialize(cellIdValueField[i]));
        }

        cellIdValueField[1] = cellIdValueField[1] + 100;

        for (int i = 0; i < cellIdValueField.Size; i++)
        {
            Console.WriteLine(JsonSerializer.Serialize(cellIdValueField[i]));
        }

        //Console.WriteLine(JsonSerializer.Serialize(cellIdValueField));

        List<Book> books =
        [
            new Book() { Name = "HP", Qty = 2, Id = "1", },
            new Book() { Name = "BEK", Qty = 200, Id = "2", },
            new Book() { Name = "LOTR", Qty = 30, Id = "3", },
        ];

        string json = JsonSerializer.Serialize(books);
        Console.WriteLine(json);
    }
}