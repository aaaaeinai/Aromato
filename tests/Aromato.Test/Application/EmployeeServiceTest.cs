﻿using System;
using System.Linq;
using Aromato.Application.Service;
using Aromato.Domain.Enumeration;
using Aromato.Infrastructure;
using Aromato.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Aromato.Test.Application
{
    public class EmployeeServiceTest
    {

        [Fact]
        public void CanCreateAndRemoveEmployee()
        {
            var firstName = "John Joshua";
            var middleName = "Remonde";
            var lastName = "Ferrer";
            var gender = Gender.Male;
            var dateOfBirth = DateTime.Parse("02/09/1996");

            using (var unitOfWork = new InMemoryUnitOfWork())
            {
                var employeeRepository = new InMemoryEmployeeRepository(unitOfWork);
                var employeeService = new EmployeeService(employeeRepository, unitOfWork);

                employeeService.CreateEmployee(firstName, lastName, middleName, gender, dateOfBirth);
            }

            using (var unitOfWork = new InMemoryUnitOfWork())
            {
                var employeeRepository = new InMemoryEmployeeRepository(unitOfWork);

                var employees = employeeRepository.FindAll();

                var employee = employees.First();

                Assert.Equal(firstName, employee.FirstName);
                Assert.Equal(middleName, employee.MiddleName);
                Assert.Equal(lastName, employee.LastName);
                Assert.Equal(gender, employee.Gender);
                Assert.Equal(dateOfBirth, employee.DateOfBirth);

                employeeRepository.Remove(employee);

                Assert.Equal(EntityState.Deleted, unitOfWork.Entry(employee).State);

                unitOfWork.Commit();
                
                Assert.Equal(EntityState.Detached, unitOfWork.Entry(employee).State);
            }

        }
    }
}