![Build Status](https://github.com/qbmary/TranslationBureau/actions/workflows/build.yml/badge.svg)

Web-приложение баз данных «Бюро переводов», разработанное в ходе изучения дисциплины «Разработка приложений баз данных для информационных систем».

## Состав решения

| Проект | Назначение |
| --- | --- |
| TranslationBureau.Domain | доменное ядро |
| TranslationBureau.Application | сценарии использования |
| TranslationBureau.Infrastructure | инфраструктура и доступ к данным |
| TranslationBureau.Web | Web-приложение ASP.NET Core MVC |
| TranslationBureau.Domain.Tests | модульные тесты |

## Требования

Для сборки и запуска требуются пакет .NET 10.0 SDK. База данных — SQLite (файл создаётся автоматически при первом запуске).

## Запуск

dotnet run --project TranslationBureau.Web

## Выполнение модульных тестов

dotnet test

