## Proste Api do zarządzania kodami promocyjnymi

### Założenia:
- Kod składa się z: nazwy, kodu (unikalnego ciągu znaków), ilości możliwych wyświetleń (pobrań),
informacji czy jest aktywny.
- Każdorazowe pobranie kodu (wyświetlenie) zmniejsza ilość możliwych wyświetleń.
- Wyświetlić można tylko kody z ilością możliwych wyświetleń większą od 0.
- Wyświetlić można tylko aktywne kody

- Kody przechowują informację o tym kiedy zostały wykorzystane
- Kody są przechowywane na prostym mongoDB stawianym na dockerze

### Aplikacja umożliwia: 
- Dodanie nowego kodu.
- Możliwość zmiany nazwy kodu promocyjnego.
- Pobranie kodu (wpływa na wyświetlenie kodu).
- Oznaczenie kodu jako nieaktywny.
- Usunięcie kodu.

- Pobranie wszystkich informacji o kodzie (Nie wpływa na ilość wyświetleń)
