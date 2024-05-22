using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

public class Ksiazka
{
    [JsonPropertyName("Id")]
    public int ID { get; set; }

    [JsonPropertyName("Tytul")]
    public string Tytul { get; set; } = string.Empty;

    [JsonPropertyName("Autor")]
    public string Autor { get; set; } = string.Empty;

    [JsonPropertyName("RokWydania")]
    public int RokWydania { get; set; }

    [JsonPropertyName("Gatunek")]
    public string Gatunek { get; set; } = string.Empty;
}

public class Ksiegarnia
{
    private List<Ksiazka> ksiazki = new List<Ksiazka>();

    // Dodaj książkę do listy
    public void DodajKsiazke()
    {
        Ksiazka nowaKsiazka = new Ksiazka();
        Console.Write("Podaj ID książki: ");
        nowaKsiazka.ID = int.Parse(Console.ReadLine() ?? "0");
        Console.Write("Podaj tytuł książki: ");
        nowaKsiazka.Tytul = Console.ReadLine() ?? string.Empty;
        Console.Write("Podaj autora książki: ");
        nowaKsiazka.Autor = Console.ReadLine() ?? string.Empty;
        Console.Write("Podaj rok wydania książki: ");
        nowaKsiazka.RokWydania = int.Parse(Console.ReadLine() ?? "0");
        Console.Write("Podaj gatunek książki: ");
        nowaKsiazka.Gatunek = Console.ReadLine() ?? string.Empty;

        ksiazki.Add(nowaKsiazka);
        Console.WriteLine("Książka została dodana.");
    }

    // Wyświetl listę książek
    public void WyswietlListeKsiazek()
    {
        Console.WriteLine("Lista książek:");
        foreach (var ksiazka in ksiazki)
        {
            Console.WriteLine($"ID: {ksiazka.ID} | Tytuł: {ksiazka.Tytul}");
        }
    }

    // Wyświetl szczegóły książki
    public void WyswietlSzczegolyKsiazki()
    {
        Console.Write("Podaj ID książki: ");
        int id = int.Parse(Console.ReadLine() ?? "0");
        var ksiazka = ksiazki.Find(k => k.ID == id);
        if (ksiazka != null)
        {
            Console.WriteLine($"ID: {ksiazka.ID}");
            Console.WriteLine($"Tytuł: {ksiazka.Tytul}");
            Console.WriteLine($"Autor: {ksiazka.Autor}");
            Console.WriteLine($"Rok Wydania: {ksiazka.RokWydania}");
            Console.WriteLine($"Gatunek: {ksiazka.Gatunek}");
        }
        else
        {
            Console.WriteLine("Książka o podanym ID nie została znaleziona.");
        }
    }

    // Usuń książkę
    public void UsunKsiazke()
    {
        Console.Write("Podaj ID książki do usunięcia: ");
        int id = int.Parse(Console.ReadLine() ?? "0");
        var ksiazka = ksiazki.Find(k => k.ID == id);
        if (ksiazka != null)
        {
            ksiazki.Remove(ksiazka);
            Console.WriteLine("Książka została usunięta.");
        }
        else
        {
            Console.WriteLine("Książka o podanym ID nie została znaleziona.");
        }
    }

    // Zapisz dane do pliku JSON
    public void ZapiszDoPliku(string sciezkaPliku)
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(ksiazki, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(sciezkaPliku, jsonString);
            Console.WriteLine("Dane zostały zapisane do pliku.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Wystąpił błąd podczas zapisywania do pliku: {ex.Message}");
        }
    }

    // Wczytaj dane z pliku JSON
    public void WczytajZPliku(string sciezkaPliku)
    {
        try
        {
            if (File.Exists(sciezkaPliku))
            {
                string jsonString = File.ReadAllText(sciezkaPliku);
                ksiazki = JsonSerializer.Deserialize<List<Ksiazka>>(jsonString) ?? new List<Ksiazka>();
                Console.WriteLine("Dane zostały wczytane z pliku.");
            }
            else
            {
                Console.WriteLine("Plik nie istnieje. Utworzony zostanie nowy plik przy zapisie.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Wystąpił błąd podczas wczytywania z pliku: {ex.Message}");
        }
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        string sciezkaPliku = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ksiazki.json");
        Ksiegarnia ksiegarnia = new Ksiegarnia();
        ksiegarnia.WczytajZPliku(sciezkaPliku);

        while (true)
        {
            Console.WriteLine("\n1. Dodaj książkę");
            Console.WriteLine("2. Wyświetl listę książek");
            Console.WriteLine("3. Wyświetl szczegóły książki");
            Console.WriteLine("4. Usuń książkę");
            Console.WriteLine("5. Zapisz zmiany i wyjdź");

            if (!int.TryParse(Console.ReadLine(), out int wybor))
            {
                Console.WriteLine("Niepoprawny wybór. Spróbuj ponownie.");
                continue;
            }

            switch (wybor)
            {
                case 1:
                    ksiegarnia.DodajKsiazke();
                    break;
                case 2:
                    ksiegarnia.WyswietlListeKsiazek();
                    break;
                case 3:
                    ksiegarnia.WyswietlSzczegolyKsiazki();
                    break;
                case 4:
                    ksiegarnia.UsunKsiazke();
                    break;
                case 5:
                    ksiegarnia.ZapiszDoPliku(sciezkaPliku);
                    return;
                default:
                    Console.WriteLine("Niepoprawny wybór. Spróbuj ponownie.");
                    break;
            }
        }
    }
}
