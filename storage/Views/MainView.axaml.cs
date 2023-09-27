using Avalonia.Controls;
using Avalonia.Interactivity;
using storage.Models;
using storage.ViewModels;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using storage.Data;

namespace storage.Views;

public partial class MainView : UserControl
{

    readonly MainViewModel _model;

    public MainView()
    {
        InitializeComponent();
        _model = new MainViewModel();
    }

    async void CreateElementClick(object sender, RoutedEventArgs e)
    {
        var btn = (Button)sender;

        switch (btn.Name)
        {
            case "CreateCategory":
                int catId = GetNextId(_model.Categories);
                var category = new Category(catId, $"Категория {catId}", "шт");
                _model.CategoryAccess.Save(category);
                _model.Categories.Add(category);
                CategoryGrid.ItemsSource = _model.Categories;
                break;
            case "CreateInvoice":
                var invoice = new Invoice(GetNextId(_model.Invoices), DateTime.Now);
                _model.InvoiceAccess.Save(invoice);
                _model.Invoices.Add(invoice);
                InvoiceGrid.ItemsSource = _model.Invoices;
                break;
            case "CreateMaterial":
                int matId = GetNextId(_model.Materials);
                var material = new Material(matId, $"Материал {matId}", GetLastEntityId(_model.Categories));
                _model.MaterialAccess.Save(material);
                _model.Materials.Add(material);
                MaterialGrid.ItemsSource = _model.Materials;
                break;
            case "CreateMaterialConsumption":
                var materialConsumption = new MaterialConsumption(
                    GetNextId(_model.MaterialConsumptions), 0, GetLastEntityId(_model.Invoices), GetLastEntityId(_model.Materials));
                _model.MaterialConsumptionAccess.Save(materialConsumption);
                _model.MaterialConsumptions.Add(materialConsumption);
                MaterialConsumptionGrid.ItemsSource = _model.MaterialConsumptions;
                break;
            case "CreateMaterialReceipt":
                var materialReceipt = new MaterialReceipt(GetNextId(_model.MaterialConsumptions), 0, GetLastEntityId(_model.Invoices), GetLastEntityId(_model.Materials));
                _model.MaterialReceiptAccess.Save(materialReceipt);
                _model.MaterialReceipts.Add(materialReceipt);
                MaterialReceiptGrid.ItemsSource = _model.MaterialReceipts;
                break;
            default:
                await ShowErrorMessage("Неверная таблица");
                break;
        }
    }

    async void Element_CellEditEnded(object? sender, DataGridCellEditEndedEventArgs e)
    {
        string errorMessage = "Данные элемента имеют неверный формат";

        if (e.EditAction != DataGridEditAction.Commit || sender is not DataGrid dataGrid)
        {
            return;
        }

        string? dataGridName = dataGrid.Name;

        switch (dataGridName)
        {
            case "CategoryGrid":
                if (CategoryGrid.SelectedItem is Category categoryElement && IsValidCategory(categoryElement))
                {
                    UpdateElement<Category>(_model.CategoryAccess, categoryElement, _model.Categories);
                }
                else
                {
                    await ShowErrorMessage(errorMessage);
                }
                break;
            case "InvoiceGrid":
                if (InvoiceGrid.SelectedItem is Invoice invoiceItem && IsValidInvoice(invoiceItem))
                {
                    UpdateElement<Invoice>(_model.InvoiceAccess, invoiceItem, _model.Invoices);
                }
                else
                {
                    await ShowErrorMessage(errorMessage);
                }
                break;
            case "MaterialGrid":
                if (MaterialGrid.SelectedItem is Material materialItem && IsValidMaterial(materialItem))
                {
                    UpdateElement<Material>(_model.MaterialAccess, materialItem, _model.Materials);
                }
                else
                {
                    await ShowErrorMessage(errorMessage);
                }
                break;
            case "MaterialConsumptionGrid":
                if (MaterialConsumptionGrid.SelectedItem is MaterialConsumption consumptionItem && IsValidMaterialConsumption(consumptionItem))
                {
                    UpdateElement<MaterialConsumption>(_model.MaterialConsumptionAccess, consumptionItem, _model.MaterialConsumptions);
                }
                else
                {
                    await ShowErrorMessage(errorMessage);
                }
                break;
            case "MaterialReceiptGrid":
                if (MaterialReceiptGrid.SelectedItem is MaterialReceipt receiptItem && IsValidMaterialReceipt(receiptItem))
                {
                    UpdateElement<MaterialReceipt>(_model.MaterialReceiptAccess, receiptItem, _model.MaterialReceipts);
                }
                else
                {
                    await ShowErrorMessage(errorMessage);
                }
                break;
        }
    }

    void DeleteElementClick(object sender, RoutedEventArgs e)
    {
        var btn = (Button)sender;

        switch (btn.Name)
        {
            case "DeleteCategory":
                DeleteElement<Category>(_model.CategoryAccess, CategoryGrid, _model.Categories);
                break;
            case "DeleteInvoice":
                DeleteElement<Invoice>(_model.InvoiceAccess, InvoiceGrid, _model.Invoices);
                break;
            case "DeleteMaterial":
                DeleteElement<Material>(_model.MaterialAccess, MaterialGrid, _model.Materials);
                break;
            case "DeleteMaterialConsumption":
                DeleteElement<MaterialConsumption>(_model.MaterialConsumptionAccess, MaterialConsumptionGrid, _model.MaterialConsumptions);
                break;
            case "DeleteMaterialReceipt":
                DeleteElement<MaterialReceipt>(_model.MaterialReceiptAccess, MaterialReceiptGrid, _model.MaterialReceipts);
                break;
        }
    }

    async void DownloadExcelReport_Click(object? sender, RoutedEventArgs e)
    {
        var excelButton = (Button)sender!;
        excelButton.IsEnabled = false;

        await Task.Run(() =>
        {
            _model.ExportToExcel();
        });

        excelButton.IsEnabled = true;
    }

    async void DownloadWordReport_Click(object? sender, RoutedEventArgs e)
    {
        var wordButton = (Button)sender!;
        wordButton.IsEnabled = false;

        await Task.Run(() =>
        {
            _model.ExportToWord();
        });

        wordButton.IsEnabled = true;
    }

    /// <summary>
    /// Метод для обновления элемента в базе данных и списке элементов.
    /// </summary>
    /// <typeparam name="T">Тип элемента.</typeparam>
    /// <param name="access">Интерфейс доступа к данным.</param>
    /// <param name="element">Элемент для обновления.</param>
    /// <param name="collection">Коллекция элементов.</param>
    void UpdateElement<T>(IAccess<T> access, T element, ICollection<T> collection)
    {
        access.Update(element);
        collection.Clear();
        var updatedCategories = access.GetAll();
        foreach (var category in updatedCategories)
        {
            collection.Add(category);
        }
    }

    /// <summary>
    /// Метод для удаления элемента из базы данных и списка элементов.
    /// </summary>
    /// <typeparam name="T">Тип элемента.</typeparam>
    /// <param name="access">Интерфейс доступа к данным.</param>
    /// <param name="dataGrid">Грид, содержащий элемент.</param>
    /// <param name="collection">Коллекция элементов.</param>
    void DeleteElement<T>(IAccess<T> access, DataGrid dataGrid, ICollection<T> collection)
    {
        var item = (T)dataGrid.SelectedItem;
        if (item == null) return;

        int itemId = GetEntityId(item);
        access.Delete(itemId);
        var itemToRemove = collection.First(entity => GetEntityId(entity) == itemId);
        collection.Remove(itemToRemove);
        dataGrid.ItemsSource = collection;
    }

    /// <summary>
    /// Получение идентификатора элемента.
    /// </summary>
    /// <typeparam name="T">Тип элемента.</typeparam>
    /// <param name="entity">Элемент для получения идентификатора.</param>
    /// <returns>Идентификатор элемента.</returns>
    int GetEntityId<T>(T entity)
    {
        var idProperty = entity?.GetType().GetProperty("Id");
        if (idProperty == null)
            throw new ArgumentException("Entity does not have a valid ID property.");
        object? idValue = idProperty.GetValue(entity);
        if (idValue != null) return Convert.ToInt32(idValue);
        throw new ArgumentException("Entity does not have a valid ID property.");
    }

    /// <summary>
    /// Получает следующий доступный идентификатор для элемента в коллекции.
    /// </summary>
    /// <typeparam name="T">Тип элемента.</typeparam>
    /// <param name="collection">Коллекция элементов.</param>
    /// <returns>Следующий доступный идентификатор.</returns>
    int GetNextId<T>(ICollection<T> collection)
    {
        return collection.Count > 0 ? collection.Max(entity => (int?)GetEntityId(entity)) + 1 ?? 1 : 1;
    }

    /// <summary>
    /// Получает последний идентификатор элемента в коллекции.
    /// </summary>
    /// <typeparam name="T">Тип элемента.</typeparam>
    /// <param name="collection">Коллекция элементов.</param>
    /// <returns>Последний идентификатор элемента.</returns>
    int GetLastEntityId<T>(ICollection<T> collection)
    {
        return collection.Count > 0 ? collection.Max(entity => (int?)GetEntityId(entity)) ?? 1 : 1;
    }

    /// <summary>
    /// Проверяет, является ли элемент категории (Category) корректным.
    /// </summary>
    /// <param name="element">Элемент категории для проверки.</param>
    /// <returns>True, если элемент корректен, иначе False.</returns>
    bool IsValidCategory(Category? element)
    {
        return element != null && !string.IsNullOrWhiteSpace(element.Name) && !string.IsNullOrWhiteSpace(element.MeasureUnit);
    }

    /// <summary>
    /// Проверяет, является ли элемент счета (Invoice) корректным.
    /// </summary>
    /// <param name="element">Элемент счета для проверки.</param>
    /// <returns>True, если элемент корректен, иначе False.</returns>
    bool IsValidInvoice(Invoice? element)
    {
        return element?.CreatedAt != null;
    }

    /// <summary>
    /// Проверяет, является ли элемент материала (Material) корректным.
    /// </summary>
    /// <param name="element">Элемент материала для проверки.</param>
    /// <returns>True, если элемент корректен, иначе False.</returns>
    bool IsValidMaterial(Material? element)
    {
        return element != null && element.CategoryId != -1 && !string.IsNullOrWhiteSpace(element.Name);
    }

    /// <summary>
    /// Проверяет, является ли элемент записи о расходе материалов (MaterialConsumption) корректным.
    /// </summary>
    /// <param name="element">Элемент записи о расходе материалов для проверки.</param>
    /// <returns>True, если элемент корректен, иначе False.</returns>
    bool IsValidMaterialConsumption(MaterialConsumption? element)
    {
        return element is { Count: > 0, InvoiceId: > 0, MaterialId: > 0 };
    }

    /// <summary>
    /// Проверяет, является ли элемент записи о поступлении материалов (MaterialReceipt) корректным.
    /// </summary>
    /// <param name="element">Элемент записи о поступлении материалов для проверки.</param>
    /// <returns>True, если элемент корректен, иначе False.</returns>
    bool IsValidMaterialReceipt(MaterialReceipt? element)
    {
        return element is { Count: > 0, InvoiceId: > 0, MaterialId: > 0 };
    }

    /// <summary>
    /// Отображает сообщение об ошибке в виде диалогового окна.
    /// </summary>
    /// <param name="message">Сообщение об ошибке.</param>
    /// <returns>Задача для асинхронного выполнения.</returns>
    async Task ShowErrorMessage(string message)
    {
        var box = MessageBoxManager.GetMessageBoxStandard("Ошибка", message, ButtonEnum.Ok);
        await box.ShowAsync();
    }
}
