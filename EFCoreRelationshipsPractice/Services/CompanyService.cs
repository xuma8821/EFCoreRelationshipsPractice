﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFCoreRelationshipsPractice.Dtos;
using EFCoreRelationshipsPractice.Model;
using EFCoreRelationshipsPractice.Repository;
using Microsoft.EntityFrameworkCore;

namespace EFCoreRelationshipsPractice.Services
{
    public class CompanyService
    {
        private readonly CompanyDbContext companyDbContext;

        public CompanyService(CompanyDbContext companyDbContext)
        {
            this.companyDbContext = companyDbContext;
        }

        public async Task<List<CompanyDto>> GetAll()
        {
            var companies = companyDbContext.Companies.Include(company => company.Profile).Include(company => company.Employees).ToList();
            return companies.Select(companyEntity => new CompanyDto(companyEntity)).ToList();
        }

        public async Task<CompanyDto> GetById(long id)
        {
            var foundCompany = companyDbContext.Companies.Include(company => company.Profile).Include(company => company.Employees).FirstOrDefault(company => company.Id == id);
            return new CompanyDto(foundCompany);
        }

        public async Task<int> AddCompany(CompanyDto companyDto)
        {
            //1. convert dto to entity
            CompanyEntity companyEntity = companyDto.ToEntity();
            //2. save entity to db
            await companyDbContext.Companies.AddAsync(companyEntity);
            await companyDbContext.SaveChangesAsync();
            //3. return company id
            return companyEntity.Id;

        }

        public async Task DeleteCompany(int id)
        {
            var foundCompany = companyDbContext.Companies.Include(company => company.Profile).Include(company => company.Employees).FirstOrDefault(company => company.Id == id);
            var targetProfile = foundCompany.Profile;
            var targetEmployees = foundCompany.Employees;
            targetEmployees.ForEach(employee => companyDbContext.Employees.Remove(employee));
            companyDbContext.Companies.Remove(foundCompany);
            companyDbContext.Profiles.Remove(targetProfile);
            await companyDbContext.SaveChangesAsync();
        }
    }
}