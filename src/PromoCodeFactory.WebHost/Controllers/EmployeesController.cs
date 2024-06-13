using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;

        public EmployeesController(IRepository<Employee> employeeRepository,IRepository<Role> roleRepository,IConfiguration configuration)
        {
            _employeeRepository = employeeRepository;
            _roleRepository = roleRepository;
            _configuration= configuration;
        }

        /// <summary>
        /// Получить данные всех сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<EmployeeShortResponse>> GetEmployeesAsync()
        {
            //var c = _configuration.GetSection("Logging:LogLevel:Default").Value;
            //var u = _configuration["SomeParam"];
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
                return NoContent(); 

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
                return BadRequest();

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

            return BadRequest();
        }

        /// <summary>
        /// Обновить данные сотрудника по Id
        /// </summary>
        [HttpPost("{id:guid}")]
        public async Task<ActionResult> UpdateEmployeeAsync(Guid id ,[FromBody] EmployeeRequest request)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return BadRequest();

            List<Role> roles = await CreateRoles(request);
            //var roles = new List<Role>();
            //foreach (var roleId in request.Roles)
            //{
            //    var role = await _roleRepository.GetByIdAsync(roleId.Id);
            //    roles.Add(role);
            //}

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
            //var roles = new List<Role>();
            //foreach (var roleId in request.Roles)
            //{
            //    var role = await _roleRepository.GetByIdAsync(roleId.Id);
            //    roles.Add(role);
            //}
            List<Role> roles = await CreateRoles(request);
            var employeeEntity = new Employee(request.FirstName, request.LastName, request.Email, request.AppliedPromocodesCount, roles);

            var result = await _employeeRepository.AddAsync(employeeEntity);
            if (result.HasValue)
                return result;
            else
            return BadRequest();
        }

        private async Task<List<Role>> CreateRoles(EmployeeRequest request)
        {
            var roles = new List<Role>();
            if (request.Roles != null)
            {
                foreach (var item in request.Roles)
                {
                    var role = await _roleRepository.GetByIdAsync(item.Id);
                    if (role == null)
                    {
                        role = new Role
                        {
                            Id = item.Id,
                            Name = item.Name,
                            Description = item.Description
                        };
                        await _roleRepository.AddAsync(role);
                    }
                    roles.Add(role);
                }
            }

            return roles;
        }
    }
}