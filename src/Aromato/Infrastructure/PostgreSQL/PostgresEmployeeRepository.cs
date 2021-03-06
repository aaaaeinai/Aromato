﻿using System.Collections.Generic;
using System.Linq;
using Aromato.Domain;
using Aromato.Domain.EmployeeAgg;
using Microsoft.EntityFrameworkCore;

namespace Aromato.Infrastructure.PostgreSQL
{
    public class PostgresEmployeeRepository : IEmployeeRepository
    {

        private readonly PostgresUnitOfWork _unitOfWork;

        public PostgresEmployeeRepository(PostgresUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Employee FindById(long id)
        {
            return _unitOfWork.Employees
                .Include(e => e.Punches)
                .Include("Roles.Role.Permissions.Permission")
                .FirstOrDefault(e => e.Id == id);
        }

        public IEnumerable<Employee> FindAll()
        {
            return _unitOfWork.Employees.AsEnumerable();
        }

        public IEnumerable<Employee> FindBySpec(ISpecification<long, Employee> specification)
        {
            return _unitOfWork.Employees
                .Include(e => e.Punches)
                .Where(specification.IsSatisified)
                .AsEnumerable();
        }

        public void Add(Employee aggregate)
        {
            _unitOfWork.Employees.Add(aggregate);
        }

        public void Remove(Employee aggregate)
        {
            _unitOfWork.Remove(aggregate);
        }

        public void RemoveById(long id)
        {
            Remove(FindById(id));
        }

        public IUnitOfWork UnitOfWork => _unitOfWork;

        public Employee FindByUniqueId(string uniqueId)
        {
            return _unitOfWork.Employees
                .Include(e => e.Punches)
                .Include("Roles.Role.Permissions.Permission")
                .First(e => e.UniqueId == uniqueId);
        }
    }
}