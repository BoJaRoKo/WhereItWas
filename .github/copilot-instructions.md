# Copilot Instructions

## General Guidelines
- W tym projekcie wszelkie nazwy zaczynające się od `Where...` w wiadomościach użytkownika należy interpretować jako odnoszące się do `WhereItWas`, chyba że użytkownik wyraźnie wskaże inaczej.

## Project-Specific Rules
- Od początku prac w WhereItWas należy ściśle pilnować ustaleń architektonicznych projektu i nie pozostawiać logiki ani dodatkowych typów w `WhereItWas.Cli` poza `Program.cs`; użytkownik oczekuje, że zmiany będą wykonywane zgodnie z tymi ustaleniami bez polegania na domyślnych decyzjach Visual Studio.
- W rozwiązaniu WhereItWas logowanie do istniejącej bazy SQL ma używać procedur SQL login/logout, connection string nie ma być wpisany na stałe, a wybór połączenia ma odbywać się przez natywne okno dialogowe z podglądem dostępnych opcji/instancji, podobne do wersji GUI MsForms, jeśli będzie to praktyczne technicznie.
- W WhereItWas cała logika bazowa ma być w `WhereItWas.Core`, ale `WhereItWas.Gui` może być szersze od `WhereItWas.Cli`, np. oferować dodatkowe uszczegółowienie i rafinację pobranych danych po stronie interfejsu z użyciem LINQ.
- W WhereItWas.Cli ma pozostać tylko jeden plik kodu `Program.cs`; cała pozostała logika i typy mają być w `WhereItWas.Core`.
- W formularzu połączenia WhereItWas pola loginu i hasła mają być widoczne tylko przy uwierzytelnianiu SQL Server, a układ przycisków i kontrolek ma być dopasowany wizualnie.
- W WhereItWas nazwy `Login`, `Logout`, `ReadConnection` i `WriteConnection` odnoszą się do metod/procedur w `WhereItWas.Core`, a nie do nazw procedur składowanych SQL; nie należy zakładać istnienia SQL `LoginMsSQL`/`LogoutMsSQL` bez wyraźnego polecenia.
- W WhereItWas formularze WinForms mają być kompletne i zdatne do edycji w projektancie Visual Studio, a nie budowane wyłącznie ręcznie w kodzie.
- W WhereItWas klasa `Execute` ma należeć do `WhereItWas.Core`, nie do `WhereItWas.Cli`. Jeśli przeniesienie wymaga współdzielonego typu parametrów, należy umieścić go tak, by uniknąć zależności kołowych.
- W WhereItWas testy jednostkowe mają być robione przede wszystkim dla `WhereItWas.Core`; testy CLI powinny być traktowane bardziej jako integracyjne wywołania programu niż podstawowy cel testów jednostkowych.