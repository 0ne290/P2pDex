namespace Core.Application.Private.Interfaces;

public interface IUnitOfWork
{
    /// <summary>
    /// Сохраняет в хранилище отслеживаемые сущности. Характер сохранения зависит от состояния сущностей в
    /// отслеживателе: "Добавлена" - сущность добавится в хранилище, "Изменена" - сущность в хранилище обновится,
    /// "Без изменений" - ничего не произойдет.
    /// </summary>
    Task SaveAllTrackedEntities();

    /// <summary>
    /// Очистить отслеживатель.
    /// </summary>
    void UntrackAllEntities();
    
    IRepository Repository { get; }
}