using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Veyrin.Core.Validation;

namespace Veyrin.Data.Infrastructure;





// TEntity：代表這個 DataAccessBase 處理的實體類型
public abstract class DataAccessBase<TEntity> where TEntity : class
{
    // protected readonly DbContext context; 是您的基礎成員
    protected readonly DbContext _context;

    // DbContext 裡的 DbSet<TEntity> 是執行資料庫操作的核心
    protected readonly DbSet<TEntity> _dbSet;

    //protected ILogger? _logger;

    public DataAccessBase(DbContext context)
    {
        Guard.NotNull(context, nameof(context));
        _context = context;
        // 獲取對應 TEntity 的 DbSet，這是 EF Core 進行操作的起點
        _dbSet = _context.Set<TEntity>();
        //_logger = logger;
    }

    // --- 【單一實體操作】 ---

    // 泛型方法：根據主鍵（Guid, int 等）獲取實體
    // Find 方法適用於根據實體主鍵查詢
    public virtual TEntity? GetById(params object[] keyValues)
    {
        return _dbSet.Find(keyValues);
    }

    // 泛型方法：添加單一實體
    public virtual void Add(TEntity entity)
    {
        _dbSet.Add(entity);
    }

    // 泛型方法：更新單一實體
    public virtual void Update(TEntity entity)
    {
        // 確保 EF Core 追蹤實體為 Modified 狀態
        _dbSet.Update(entity);
    }

    // 泛型方法：刪除單一實體
    public virtual void Delete(TEntity entity)
    {
        _dbSet.Remove(entity);
    }

    // --- 【多實體和查詢操作】 ---

    // 獲取所有實體
    public virtual IQueryable<TEntity> GetAll()
    {
        return _dbSet.AsNoTracking(); // 預設使用 AsNoTracking 提高讀取性能
    }

    // 根據條件查詢實體 (允許延遲執行，返回 IQueryable)
    // Expression<Func<TEntity, bool>> 是 LINQ 查詢的關鍵，它會被轉換為 SQL
    public virtual IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
    {
        return _dbSet.Where(predicate).AsNoTracking();
    }

    // --- 【持久化操作】 ---

    // 提交變更到資料庫
    public virtual int Commit()
    {
        // 您可以將 SaveChanges() 放在這裡，統一處理例外或日誌記錄
        return _context.SaveChanges();
    }
}