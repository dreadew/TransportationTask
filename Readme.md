# Решение транспортной задачи

Приложение написано на языке C# с применением технологий параллельного программирования.

### Содержание
- [Обзор](#обзор)
- [Функциональные возможности](#функциональные-возможности)
- [Используемые технологии](#используемые-технологии)
- [Формат входных и выходных данных](#формат-входных-и-выходных-данных)
- [CI/CD Конвейер](#cicd-конвейер)

## Обзор

Данное приложение предназначено для поиска оптимального плана перевозок с минимальными затратами при транспортировке товаров от нескольких поставщиков к нескольким потребителям. Оно использует различные алгоритмы и параллельные вычисления для эффективной обработки больших объемов данных (например, 1000x1000 поставщиков/потребителей).

Программа проходит 6 начальных тестов, используя входные данные из файлов `data/in[i].txt` и проверяя их с ожидаемыми результатами из файлов `expected/out[i].txt`. Проверка включает сравнение вычисленных результатов с эталонными данными. В случае успешного прохождения тестов выводится подтверждение корректности алгоритма.

Результаты выполнения тестов можно просмотреть в разделе Actions данного репозитория, где каждый шаг CI/CD конвейера подробно отображает логи выполнения, результаты тестов и выявленные ошибки, если они имеются.

## Инструкция по запуску программы с использованием аргументов командной строки

### С указанием входных и выходных данных:
```
dotnet run --project TransportationTask.csproj data/in1.txt data/out1.txt
```
В этом случае:
- Входные данные (`args[0]`): `data/in1.txt`
- Выходные данные (`args[1]`): `data/out1.txt`

### С указанием только входных данных:
```
dotnet run --project TransportationTask.csproj data/in1.txt
```
В этом случае:
- Входные данные (`args[0]`): `data/in1.txt`

### Без аргументов командной строки:
```
dotnet run --project TransportationTask.csproj
```
В этом случае:
- Получение входных данных происходит из `data/in.txt`
- Выходные данные записываются в `data/out.txt`

## Функциональные возможности

- Реализация нескольких методов для нахождения начального опорного плана:
  - Метод северо-западного угла
  - Метод наименьшей стоимости
  - Метод аппроксимации Фогеля
- Оптимизация начального решения с помощью метода потенциалов (MODI)
- Использование параллельного программирования (Task Parallel Library) для ускорения вычислений
- Соблюдение принципов ООП для удобства поддержки и расширения
- Включение CI/CD конвейера с использованием GitHub Actions

## Используемые технологии
- Язык программирования: C# (.NET 7.0)
- CI/CD: GitHub Actions

## Формат входных и выходных данных

### Формат входные данных (`data/in.txt`):
- Первая строка: два числа N и M, обозначающие количество поставщиков и потребителей
- Вторая строка: запасы каждого поставщика `A1 A2 ... An`
- Третья строка: потребности каждого потребителя `B1 B2 ... Bn`
- N строк: стоимости перевозок от каждого поставщика к каждому потребителю

### Пример входных данных
```
3 3
30 40 20
20 30 40
2 3 1
5 4 8
5 6 8
```

### Формат выходных данных (`data/out.txt`):
- Первая строка: общая минимальная стоимость перевозок
- N строк: матрица плана перевозок `Xij`


### Пример выходных данных
```
220
20 0 10
0 30 10
0 0 20
```

## CI/CD Конвейер

Проект включает CI/CD конвейер, настроенный с использованием GitHub Actions. Рабочий процесс определен в .github/workflows/dotnet.yml и выполняет следующие шаги:
- Клонирование кода
- Настройка среды .NET
- Восстановление зависимостей
- Сборка проекта
- Запуск исходных тестов
- Публикация артефактов сборки
