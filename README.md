Paskirstytas žodžių analizatorius (C#)

Ši programa sukurta kaip paskirstyta sistema, susidedanti iš trijų atskirų C# konsolinių projektų:

- Master – priima duomenis iš agentų per Named Pipes, apdoroja ir pateikia bendrą rezultatą
- ScannerA – analizuoja .txt failus nurodytame kataloge ir siunčia žodžių dažnį į Master
- ScannerB – veikia kaip antras agentas, veikiantis atskirame CPU branduolyje

Funkcionalumas

- .txt failų paieška ir nuskaitymas
- Žodžių dažnio analizė
- Duomenų siuntimas naudojant NamedPipeClientStream
- Duomenų priėmimas Master procese per NamedPipeServerStream
- Gijų valdymas su Thread ir ProcessorAffinity

Versijų istorija 

1 Pradinė versija – įkelta visa projekto struktūra (Master, ScannerA, ScannerB)
2 Pridėta FailoZodis klasė – žodžių analizės duomenų struktūra
3 Įgyvendintas NamedPipe bendravimas tarp Master ir agentų
4 Įtrauktas Thread naudojimas ir ProcessorAffinity branduolių valdymas
5 Įdėta .txt failų paieška, filtravimas ir rezultatų išvedimas

Sukompiliuota naudojant:
.NET 6.0
Visual Studio 2022

Projekto struktūra

/Master
/ScannerA
/ScannerB
README.md

Autorius

Gabrielė Vasilenko
