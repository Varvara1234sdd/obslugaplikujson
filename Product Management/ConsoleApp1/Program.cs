using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace SklepConsoleApp
{
    public class Produkt
    {
        public int Id { get; set; }
        public string Nazwa { get; set; }
        public string Producent { get; set; }
        public decimal Cena { get; set; }
        public string Kategoria { get; set; }
        public int Ilosc { get; set; }
        public DateTime DataDostawy { get; set; }
    }

    public enum SposobDostawy
    {
        OdbiorOsobisty,
        Kurier
    }

    public enum SposobPlatnosci
    {
        Karta,
        Gotowka
    }

    public class Zamowienie
    {
        public List<Tuple<Produkt, int>> ListaProduktow { get; set; } = new List<Tuple<Produkt, int>>();
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public string Adres { get; set; }
        public SposobDostawy SposobDostawy { get; set; }
        public SposobPlatnosci SposobPlatnosci { get; set; }
        public decimal KwotaCalkowita { get; private set; }

        public void DodajProdukt(Produkt produkt, int ilosc)
        {
            ListaProduktow.Add(new Tuple<Produkt, int>(produkt, ilosc));
            ObliczKwoteCalkowita();
        }

        private void ObliczKwoteCalkowita()
        {
            KwotaCalkowita = 0;
            foreach (var item in ListaProduktow)
            {
                KwotaCalkowita += item.Item1.Cena * item.Item2;
            }

            // Dodaj opłaty za sposób dostawy i płatności
            switch (SposobDostawy)
            {
                case SposobDostawy.OdbiorOsobisty:
                    // Brak dodatkowych opłat
                    break;
                case SposobDostawy.Kurier:
                    KwotaCalkowita += 20; // Dodaj opłatę za kuriera
                    break;
            }

            switch (SposobPlatnosci)
            {
                case SposobPlatnosci.Karta:
                    KwotaCalkowita += 2; // Dodaj opłatę za płatność kartą
                    break;
                case SposobPlatnosci.Gotowka:
                    // Brak dodatkowych opłat
                    break;
            }
        }

        public override string ToString()
        {
            string szczegoly = $"Imię: {Imie}\n" +
                               $"Nazwisko: {Nazwisko}\n" +
                               $"Adres: {Adres}\n" +
                               $"Sposób dostawy: {SposobDostawy}\n" +
                               $"Sposób płatności: {SposobPlatnosci}\n" +
                               $"Kwota całkowita: {KwotaCalkowita}\n" +
                               "Lista produktów:\n";

            foreach (var item in ListaProduktow)
            {
                szczegoly += $"- {item.Item1.Nazwa}, ilość: {item.Item2}, cena: {item.Item1.Cena}\n";
            }

            return szczegoly;
        }
    }

    public class Sklep
    {
        private List<Produkt> Produkty { get; set; }
        public List<Zamowienie> Zamowienia { get; private set; } = new List<Zamowienie>();

        public Sklep()
        {
            Produkty = WczytajProdukty();
        }

        private List<Produkt> WczytajProdukty()
        {
            var json = File.ReadAllText("produkty.json");
            return JsonSerializer.Deserialize<List<Produkt>>(json);
        }

        public void DodajProduktDoZamowienia(Zamowienie zamowienie, int produktId, int ilosc)
        {
            var produkt = Produkty.FirstOrDefault(p => p.Id == produktId);
            if (produkt != null)
            {
                zamowienie.DodajProdukt(produkt, ilosc);
            }
            else
            {
                Console.WriteLine("Produkt o podanym ID nie istnieje.");
            }
        }

        public void ZapiszZamowienia()
        {
            var json = JsonSerializer.Serialize(Zamowienia, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("zamowienia.json", json);
        }

        public void WyswietlProdukty()
        {
            Console.WriteLine("Dostępne produkty:");
            foreach (var produkt in Produkty)
            {
                Console.WriteLine($"ID: {produkt.Id}, Nazwa: {produkt.Nazwa}, Cena: {produkt.Cena}, Ilość: {produkt.Ilosc}");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Sklep sklep = new Sklep();

            Console.WriteLine("Witaj w sklepie!");

            Zamowienie zamowienie = new Zamowienie();

            Console.Write("Podaj imię: ");
            zamowienie.Imie = Console.ReadLine();

            Console.Write("Podaj nazwisko: ");
            zamowienie.Nazwisko = Console.ReadLine();

            Console.Write("Podaj adres: ");
            zamowienie.Adres = Console.ReadLine();

            Console.WriteLine("Wybierz sposób dostawy: 1 - Odbiór osobisty, 2 - Kurier");
            int sposobDostawy = int.Parse(Console.ReadLine());
            zamowienie.SposobDostawy = (SposobDostawy)(sposobDostawy - 1);

            Console.WriteLine("Wybierz sposób płatności: 1 - Karta, 2 - Gotówka");
            int sposobPlatnosci = int.Parse(Console.ReadLine());
            zamowienie.SposobPlatnosci = (SposobPlatnosci)(sposobPlatnosci - 1);

            bool dodawanieProduktow = true;
            while (dodawanieProduktow)
            {
                sklep.WyswietlProdukty();

                Console.Write("Podaj ID produktu, który chcesz dodać do zamówienia: ");
                int produktId = int.Parse(Console.ReadLine());

                Console.Write("Podaj ilość: ");
                int ilosc = int.Parse(Console.ReadLine());

                sklep.DodajProduktDoZamowienia(zamowienie, produktId, ilosc);

                Console.WriteLine("Czy chcesz dodać kolejny produkt? (tak/nie)");
                string odpowiedz = Console.ReadLine().ToLower();
                if (odpowiedz != "tak")
                {
                    dodawanieProduktow = false;
                }
            }

            Console.WriteLine(zamowienie);

            sklep.Zamowienia.Add(zamowienie);
            sklep.ZapiszZamowienia();

            Console.WriteLine("Zamówienie zostało zapisane.");
        }
    }
}
