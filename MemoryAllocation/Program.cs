using System.Diagnostics;

namespace MemoryAllocation
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Öt folyamatunk van
            const int processCount = 5;

            // Négy lapunk van
            const int pageCount = 4;

            // A folyamatváltások számát tároló változó
            int contextSwitches = 0;

            // A laphibákat tároló tömb
            int[] pageErrors = new int[pageCount];

            // A folyamatváltásokat leíró szöveges állomány beolvasása
            string[] contextSwitchFile = File.ReadAllLines("context_switches.txt");

            // A lapok állapotát tároló tömb
            int[] pages = new int[pageCount];

            // A folyamatok állapotát tároló tömb
            int[] processes = new int[processCount];

            // A folyamatokat és lapokat a kezdeti állapotukhoz állítjuk
            for (int i = 0; i < processCount; i++)
            {
                processes[i] = -1;
            }
            for (int i = 0; i < pageCount; i++)
            {
                pages[i] = -1;
            }

            // A folyamatváltásokat leíró szöveges állomány sorainak végigmenésével
            for (int i = 0; i < contextSwitchFile.Length; i++)
            {
                // Az aktuális folyamat azonosítójának beolvasása
                int processId = int.Parse(contextSwitchFile[i]);

                // Ha az aktuális folyamat még nem futott, akkor elindítjuk
                if (processes[processId] == -1)
                {
                    // Keresünk egy üres lapot
                    int pageId = -1;
                    for (int j = 0; j < pageCount; j++)
                    {
                        if (pages[j] == -1)
                        {
                            pageId = j;
                            break;
                        }
                    }

                    // Ha nem találtunk üres lapot, akkor a FIFO algoritmus segítségével választunk ki egy lapot
                    if (pageId == -1)
                    {
                        // A legkorábban indított folyamat azonosítóját keressük
                        int oldestProcessId = -1;
                        for (int j = 0; j < processCount; j++)
                        {
                            if (processes[j] != -1)
                            {
                                if (oldestProcessId == -1 || processes[j] < processes[oldestProcessId])
                                {
                                    oldestProcessId = j;
                                }
                            }
                        }

                        // A legkorábban indított folyamat lapját választjuk ki
                        pageId = processes[oldestProcessId];
                    }

                    // A folyamatot elindítjuk a kiválasztott lapra
                    processes[processId] = pageId;
                    pages[pageId] = processId;

                    // Növeljük a folyamatváltások számát
                    contextSwitches++;

                    // Kiírjuk az aktuális lapok állapotát és a folyamatváltások számát
                    Console.WriteLine("Context switches: " + contextSwitches);
                    Console.WriteLine("Pages: " + string.Join(", ", pages));

                    // Növeljük a laphibák számát az összes lap esetében
                    for (int j = 0; j < pageCount; j++)
                    {
                        pageErrors[j]++;
                    }
                }
            }
        }
    }
}