﻿using shuttleasy.DAL.EFRepositories;
using shuttleasy.DAL.EFRepositories.CompanyWorkers;
using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics.CompanyWorkers
{
    public class CompanyWorkerLogic : ICompanyWorkerLogic
    {
        private ICompanyWorkerRepository _companyWorkerRepository;
        public CompanyWorkerLogic(ICompanyWorkerRepository companyWorkerRepository)
        {
            _companyWorkerRepository = companyWorkerRepository;
        }

        public bool Add(CompanyWorker companyWorker)
        {
            bool isAdded = _companyWorkerRepository.Add(companyWorker);
            return isAdded;

        }
        public CompanyWorker? GetCompanyWorkerWithEmail(string email) // yav buralara try catch yazmak lazım ama ne döndüreceğimi bilmiyom
        {
            Func<CompanyWorker, bool> getCompanyWorker = getCompanyWorker => getCompanyWorker.Email == email;
            CompanyWorker? companyWorker = _companyWorkerRepository.GetSingle(getCompanyWorker);
            return companyWorker;

        }

        public bool UpdateDriverWithEmail(CompanyWorker updatedCompanyWorker, string email)
        {
            Func<CompanyWorker, bool> getDriver = pas => pas.Email == email;
            bool isUpdated = _companyWorkerRepository.Update(updatedCompanyWorker, getDriver);
            return isUpdated;        
        }
    }
}