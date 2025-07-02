using Microsoft.EntityFrameworkCore;
using SurveyProject.DataAccess.Abstract;
using SurveyProject.DataAccess.Context;
using System.Linq.Expressions;

namespace SurveyProject.DataAccess.Repositories
{

    /// <summary>
    /// IGenericDal arayüzünü Entity Framework Core kullanarak uygular.
    /// Veritabanı işlemlerini merkezi bir yerden yönetir.
    /// </summary>
    /// <typeparam name="T">İşlem yapılacak entity (varlık) türü.</typeparam>
    public class GenericRepository<T> : IGenericDal<T> where T : class
    {
        // DbContext sınıfı, veritabanı ile olan oturumu temsil eder.
        protected readonly AppDbContext _context;

        // DbSet, veritabanındaki bir tabloyu temsil eder.
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public void Insert(T entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();
            // Değişiklikleri kaydetmek genellikle Unit of Work deseninde SaveChanges() metodu ile yapılır.
        }

        public void Update(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();

        }

        public void Delete(T entity)
        {
            // Varlık veritabanından silinmek yerine "IsDeleted" gibi bir property ile pasif hale de getirilebilir (Soft Delete).
            _dbSet.Remove(entity);
            _context.SaveChanges();

        }

        public T GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public List<T> GetListAll()
        {
            return _dbSet.ToList();
        }

        public List<T> GetListAll(Expression<Func<T, bool>> filter)
        {
            return _dbSet.Where(filter).ToList();
        }
    }
}
