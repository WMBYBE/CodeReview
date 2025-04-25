using System;
using System.Collections.Generic;
using SportsPro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;



public class Repository<T> : IRepository<T> where T : class
{
    private readonly SportsProContext _context;
    private readonly DbSet<T> _entities;

    public Repository(SportsProContext context)
    {
        _context = context;
        _entities = context.Set<T>();
    }

    public IEnumerable<T> GetAll() => _entities.ToList();

    public T GetById(int id) => _entities.Find(id);

    public void Add(T entity)
    {
        _entities.Add(entity);
        _context.SaveChanges();
    }

    public void Update(T entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var entity = _entities.Find(id);
        if (entity != null)
        {
            _entities.Remove(entity);
            _context.SaveChanges();
        }
    }
}
