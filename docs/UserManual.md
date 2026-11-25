# WhereItWas — podręcznik użytkownika

## 1. Przeznaczenie

WhereItWas wyszukuje pliki i katalogi zapisane w bazie katalogowej SQL Server. Wynik opisuje stan zapisany podczas skanowania, a nie bieżący stan dysku. Program nie skanuje dysków i nie aktualizuje katalogu.

## 2. Połączenie z bazą

Uruchom WhereItWas.Gui w systemie Windows. Program wymaga .NET 8 i dostępnej bazy SQL Server zawierającej widok `dbo.TreeNodeSearch` o strukturze zgodnej z generatorem zapytań.

Jeśli zapisany profil połączenia działa, program używa go automatycznie. W przeciwnym przypadku otwiera okno logowania. Wskaż serwer, bazę oraz tryb uwierzytelniania: Windows albo SQL Server (login i hasło). Możesz odświeżyć listę sugerowanych serwerów, pobrać listę baz, przetestować połączenie i zapamiętać profil. Anulowanie logowania zamyka program.

## 3. Pierwsze wyszukiwanie

1. W górnym polu wpisz `*.pdf|f`.
2. Wybierz wariant `Tylko ścieżka`.
3. Naciśnij Generate, aby utworzyć zapytania SQL.
4. Naciśnij Execute, aby wykonać wybrany wariant.
5. Wyniki pojawią się na zakładce Dane. Zakładka SQL pokazuje zapytanie z ostatniego Generate.

Po zmianie wzorca naciśnij ponownie Generate. Po zmianie wariantu również warto nacisnąć Generate, aby odświeżyć podgląd SQL. Execute wybiera wariant na podstawie aktualnej listy wyboru, ale korzysta z ostatniego poprawnie wygenerowanego zestawu zapytań. Edycja tekstu na zakładce SQL nie zmienia wykonywanego zapytania.

## 4. Język zapytań — składnia i semantyka

### 4.1. Wzorce nazw

| Zapis | Znaczenie |
| --- | --- |
| `abc` | Dokładna pełna nazwa abc. |
| `abc*` | Nazwa rozpoczynająca się od abc. |
| `*abc` | Nazwa kończąca się na abc. |
| `*abc*` | Nazwa zawierająca abc. |
| `?` | Dokładnie jeden znak. |
| `*` | Dowolna nazwa (brak warunku na nazwę). |
| `<abc>` | Jeden znak z podanego zbioru. |
| `<0-9>` | Jeden znak z zakresu cyfr. |
| `<^abc>` | Jeden znak spoza zbioru abc, zgodnie z SQL LIKE. |

Przykład: `raport<0-9>?.pdf` — nazwa raport, cyfra, jeden dowolny znak i rozszerzenie pdf.

Porównania wykonuje SQL Server przy użyciu = lub LIKE. Rozróżnianie wielkości liter, znaków diakrytycznych oraz interpretacja zakresów znaków zależą od ustawień sortowania (COLLATION) odpowiednich kolumn.

### 4.2. Nazwa i rozszerzenie

Bez niecytowanej kropki wzorzec dotyczy pełnej nazwy (`Name`). Ostatnia niecytowana kropka poza klasą znaków rozdziela wzorzec na `NameWithoutExtension` i `Extension`.

| Zapis | Znaczenie |
| --- | --- |
| `*.pdf` | Dowolna nazwa, rozszerzenie pdf. |
| `raport.*` | Nazwa bez rozszerzenia raport, dowolne rozszerzenie (także puste). |
| `raport.` | Nazwa raport, puste rozszerzenie. |
| `.*` | Pusta nazwa bez rozszerzenia, dowolne rozszerzenie. |
| `a.b.txt` | Nazwa bez rozszerzenia a.b, rozszerzenie txt. |

### 4.3. Łańcuch ścieżki

Separator `/` oznacza bezpośrednie powiązanie rodzic–dziecko, nie dowolną głębokość. Elementy zapisujemy od rodzica do dziecka. Bez początkowego `/` dopasowanie może zacząć się na dowolnym poziomie drzewa.

| Zapis | Znaczenie |
| --- | --- |
| `projekty/*.cs` | Pliki lub katalogi z rozszerzeniem cs bezpośrednio pod węzłem projekty. |
| `projekty/*/*.cs` | Jak wyżej, ale z jednym pośrednim węzłem. |
| `/` | Właściwe korzenie zapisane w bazie (LevelInTree = 0, ParentId nie jest NULL). |
| `/*/projekty` | Węzeł projekty bezpośrednio pod dowolnym właściwym korzeniem. |

`**` nie jest operatorem rekurencyjnym: sam ciąg gwiazdek oznacza dowolny pojedynczy element. Końcowy `/` i puste elementy wewnątrz ścieżki są niedozwolone.

### 4.4. Typ ostatniego węzła

| Przyrostek | Znaczenie |
| --- | --- |
| `|f` | Plik (`File`). |
| `|d` | Katalog (`Directory`). |
| `|a` | Dowolny typ (domyślny również bez przyrostka). |

Litery typu mogą być wielkie lub małe. Filtr dotyczy ostatniego elementu ścieżki, nawet gdy zwracamy inny element za pomocą `:`.

### 4.5. Wybór zwracanego węzła

Domyślnie zwracany jest ostatni element ścieżki. Jeden znak `:` przed wybranym elementem pozwala zwrócić wcześniejszy węzeł. Wyniki projekcji na przodka są pozbawiane duplikatów przez SQL DISTINCT.

Przykłady:

- `projekty/*.cs|f` — znalezione pliki cs.
- `:projekty/*.cs|f` — węzły projekty zawierające bezpośrednio co najmniej jeden plik cs.
- `:*/README.md|f` — rodzice plików README.md.

### 4.6. Znaki specjalne i cytowanie

Znak `\` cytuje następny znak. Na przykład `a\.b` dopasowuje pełną nazwę a.b bez rozdzielania jej na nazwę i rozszerzenie. `C\:` oznacza literalny dwukropek, a `\\` literalny ukośnik odwrotny. W zapytaniach używaj `/` jako separatora elementów, nie separatora Windows `\`.

Znaki `%`, `_` i `[` poza klasą znaków są traktowane literalnie. Wewnątrz `<...>` zawartość jest przekazywana do klasy znaków SQL LIKE; nie wszystkie nietypowe klasy są walidowane przez parser.

### 4.7. Formalny zapis struktury

```text
zapytanie = ścieżka [ "|" typ ]
typ       = "f" | "d" | "a" (bez rozróżniania wielkości liter)
ścieżka   = "/" | [ "/" ] element { "/" element }
element   = [ ":" ] wzorzec
wzorzec   = niepusty ciąg literałów, znaków cytowanych, *, ? i klas <...>
```

Dodatkowe warunki: maksymalnie jeden niecytowany `|` poza klasą i jeden znacznik projekcji `:`; ostatnia niecytowana kropka rozdziela nazwę i rozszerzenie (każda z tych części może być pusta). Klasy nie mogą być puste ani zagnieżdżone. Nie wolno pozostawić niedomkniętej klasy, samotnego `>` ani końcowego znaku cytowania. Parser nie przycina automatycznie spacji. Sam pusty tekst jest odrzucany.

## 5. Warianty wyników

| Wariant | Wynik |
| --- | --- |
| Pola | Wszystkie kolumny znalezionych węzłów. |
| Pola ze ścieżką | Kolumny węzłów oraz FullPath. |
| Tylko ścieżka | Wyłącznie FullPath. |
| Liczba | Liczba zwracanych węzłów, bez limitu. |

Ścieżka jest rekonstruowana po relacjach ParentId aż do właściwego korzenia. Jeśli takiego korzenia brakuje, dany węzeł może wystąpić w wariancie Pola, ale nie w wariantach ze ścieżką. Liczba zlicza wynik wariantu Pola. GUI dodaje kolumnę Lp. do tabeli wyników.

## 6. Pomoc i ograniczenia

- F1 w głównym oknie otwiera ten podręcznik w oknie pomocy z formatowaniem.
- Najechanie kursorem na kontrolkę pokazuje podpowiedź; opis wariantu pochodzi z katalogu zapytań Core.
- GUI nie udostępnia obecnie limitu wyników ani konfigurowania ORDER BY. Przy bardzo szerokich wzorcach zacznij od wariantu Liczba.
- Domyślny timeout polecenia SQL wynosi 120 sekund. Rekurencyjne odtwarzanie ścieżek podlega domyślnemu limitowi rekurencji SQL Server.
- W przypadku błędu Generate popraw wzorzec i wygeneruj zapytanie ponownie przed Execute; poprzedni zestaw zapytań może pozostać w pamięci.
- CLI jest niedokończone (wywołanie wykonujące zapytanie jest zakomentowane). Podręcznik opisuje GUI i parser Core.

Podstawa: WhereItWas.Core/Execute.cs, SqlSessionManager.cs, ConnectionDialog.cs oraz WhereItWas.Gui/Form1.cs. Opis odpowiada implementacji, nie planowanym funkcjom.
