using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Сотрудники
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IRepository<Employee> _employeeRepository;

        private readonly IRepository<Role> _roleRepository;

        public EmployeesController(IRepository<Employee> employeeRepository,IRepository<Role> roleRepository)
        {
            _employeeRepository = employeeRepository;
            _roleRepository = roleRepository;
        }

        /// <summary>
        /// Получить данные всех сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<EmployeeShortResponse>> GetEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();

            var employeesModelList = employees.Select(x =>
                new EmployeeShortResponse()
                {
                    Id = x.Id,
                    Email = x.Email,
                    FullName = x.FullName,
                }).ToList();

            return employeesModelList;
        }

        /// <summary>
        /// Получить данные сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            var employeeModel = new EmployeeResponse()
            {
                Id = employee.Id,
                Email = employee.Email,
                Roles = employee.Roles.Select(x => new RoleItemResponse()
                {
                    Id=x.Id,
                    Name = x.Name,
                    Description = x.Description
                }).ToList(),
                FullName = employee.FullName,
                AppliedPromocodesCount = employee.AppliedPromocodesCount
            };

            return employeeModel;
        }

        /// <summary>
        /// Удалить данные сотрудника по Id
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            var employeeModel = new EmployeeResponse()
            {
                Id = employee.Id,
                Email = employee.Email,
                Roles = employee.Roles.Select(x => new RoleItemResponse()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description
                }).ToList(),
                FullName = employee.FullName,
                AppliedPromocodesCount = employee.AppliedPromocodesCount
            };
            var success = await _employeeRepository.DeleteByIdAsync(id);
            if (success)
            {
                return Ok(employeeModel);
            }

            return NotFound();
        }

        /// <summary>
        /// Обновить данные сотрудника по Id
        /// </summary>
        [HttpPost("{id:guid}")]
        public async Task<ActionResult> UpdateEmployeeAsync(Guid id ,[FromBody] EmployeeRequest request)
        {
            var roles = new List<Role>();
            foreach (var roleId in request.Roles)
            {
                var role = await _roleRepository.GetByIdAsync(roleId.Id);
                roles.Add(role);
            }

            var employeeEntity = new Employee(id, request.FirstName, request.LastName, request.Email, request.AppliedPromocodesCount, roles);

            var result = await _employeeRepository.UpdateAsync(id,employeeEntity);
            if (result)
                  return Ok();
            else
                return  BadRequest(); 
        }

        /// <summary>
        /// Добавить нового сотрудника
        /// </summary>
        [HttpPost]
        //[Route("Create")]
        public async Task<ActionResult<Guid?>> CreateEmployeeAsync(EmployeeRequest request)
        {
            var roles = new List<Role>();
            foreach (var roleId in request.Roles)
            {
                var role = await _roleRepository.GetByIdAsync(roleId.Id);
                roles.Add(role);
            }

            var employeeEntity = new Employee(request.FirstName, request.LastName, request.Email, request.AppliedPromocodesCount, roles);

            var result = await _employeeRepository.AddAsync(employeeEntity);
            if (result.HasValue)
                return result;

            return Ok();
        }
    }
}