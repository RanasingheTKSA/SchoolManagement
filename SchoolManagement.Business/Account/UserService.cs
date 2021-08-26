﻿using Microsoft.Extensions.Configuration;
using SchoolManagement.Business.Interfaces.AccountData;
using SchoolManagement.Data.Data;
using SchoolManagement.Master.Data.Data;
using SchoolManagement.Model;
using SchoolManagement.Util;
using SchoolManagement.Util.Constants;
using SchoolManagement.ViewModel.Account;
using SchoolManagement.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManagement.Business
{
    public class UserService: IUserService
    {
        private readonly MasterDbContext masterDb;
        private readonly SchoolManagementContext schoolDb;
        private readonly IConfiguration config;
        private readonly ICurrentUserService currentUserService;

        public UserService(MasterDbContext masterDb, SchoolManagementContext schoolDb, IConfiguration config, ICurrentUserService currentUserService)
        {
            this.masterDb = masterDb;
            this.schoolDb = schoolDb;
            this.config = config;
            this.currentUserService = currentUserService;
        }

        public async Task<ResponseViewModel> DeleteUser(int id)
        {
            var response = new ResponseViewModel();
            try
            {
                var user = schoolDb.Users.FirstOrDefault(x => x.Id == id);

                user.IsActive = false;
 
                schoolDb.Users.Update(user);
                await schoolDb.SaveChangesAsync();

                response.IsSuccess = true;
                response.Message = "User has been deleted.";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Error occured. Please try again.";
            }

            return response;
        }

        public UserViewModel GetUserbyId(int id)
        {
            var response = new UserViewModel();

            var user = schoolDb.Users.FirstOrDefault(x => x.Id == id);


            response.Id = user.Id;
            response.FullName = user.FullName;
            response.Username = user.Username;
            response.Address = user.Address;
            response.Address = user.Email;
            response.MobileNo = user.MobileNo;
            response.IsActive = user.IsActive;

            var assignedRoles = user.UserRoles.Where(x => x.IsActive == true);

            foreach (var item in assignedRoles)
            {
                response.Roles.Add(new DropDownViewModel() { Id = item.RoleId, Name = item.Role.Name });
            }

            return response;
        }

        public List<UserViewModel> GetAllUsers(int roleId)
        {
            var response =  new List<UserViewModel>();

            var query = schoolDb.Users.Where(u => u.IsActive == true);

            if (roleId > 0)
            {
                query = query.Where(x => x.UserRoles.Any(x => x.RoleId == roleId)).OrderBy(x => x.FullName);
            }

            var userList = query.ToList();

            foreach(var user in userList)
            {
                var vm = new UserViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Username = user.Username,
                    Address = user.Address,
                    Email = user.Email,
                    MobileNo = user.MobileNo,
                    
                };

                response.Add(vm);
            }

            return response;
        }



        public async Task<ResponseViewModel> SaveUser(UserViewModel vm, string userName)
        {
            var response = new ResponseViewModel();

            try
            {
                var loggedInUser = currentUserService.GetUserByUsername(userName);

                var user = schoolDb.Users.FirstOrDefault(x => x.Id == vm.Id);
              
                var getUserRoles = schoolDb.Roles.FirstOrDefault(x => x.IsActive == true);
             

                if (user == null)
                {
                    user = new User()
                    {
                        Id = vm.Id,
                        FullName = vm.FullName,
                        Email = vm.Email,
                        MobileNo = vm.MobileNo,
                        Username = vm.Username,
                        Address = vm.Address,
                        Password = CustomPasswordHasher.GenerateHash(vm.Password),
                        IsActive = true,
                        CreatedOn = DateTime.UtcNow,
                        CreatedById = loggedInUser.Id,
                        UpdatedOn = DateTime.UtcNow,
                        UpdatedById = loggedInUser.Id
                    };

                    user.UserRoles = new HashSet<UserRole>();

                    foreach (var item in vm.Roles)
                    {
                        var userRole = new UserRole()
                        {
                            RoleId = item.Id,
                            IsActive = true,
                            CreatedById = loggedInUser.Id,
                            CreatedOn = DateTime.UtcNow,
                            UpdatedById = loggedInUser.Id,
                            UpdatedOn = DateTime.UtcNow
                        };

                        user.UserRoles.Add(userRole);
                    }

                    schoolDb.Users.Add(user);
                    //(var item in vm.Roles.Where(x => x.Id))
                    response.IsSuccess = true;
                    response.Message = UserServiceConstants.NEW_USER_SAVE_SUCCESS_MESSAGE;
                }
                else
                {
                    user.Address = vm.Address;
                    user.FullName = vm.FullName;
                    user.Email = vm.Email;
                    user.MobileNo = vm.MobileNo;
                    user.UpdatedById = loggedInUser.Id;
                    user.UpdatedOn = DateTime.UtcNow;

                    var existingRoles = user.UserRoles.ToList();
                    var selectedRols = vm.Roles.ToList();

                    var newRoles = (from r in selectedRols where !existingRoles.Any(x => x.RoleId == r.Id) select r).ToList();

                    var deletedRoles = (from r in existingRoles where selectedRols.Any(x => x.Id == r.RoleId) select r).ToList();

                    foreach (var item in newRoles)
                    {
                        var userRole = new UserRole()
                        {
                            RoleId = item.Id,
                            IsActive = true,
                            CreatedById = loggedInUser.Id,
                            CreatedOn = DateTime.UtcNow,
                            UpdatedById = loggedInUser.Id,
                            UpdatedOn = DateTime.UtcNow
                        };

                        user.UserRoles.Add(userRole);
                    }

                    foreach (var deletedRole in deletedRoles)
                    {
                        user.UserRoles.Remove(deletedRole);
                    }

                    schoolDb.Users.Update(user);

                    response.IsSuccess = true;
                    response.Message = UserServiceConstants.EXISTING_USER_SAVE_SUCCESS_MESSAGE;

                }

                await schoolDb.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = UserServiceConstants.USER_SAVE_EXCEPTION_MESSAGE;
            }
            return response;
        }


        public List<DropDownViewModel> GetAllRoles()
        {
            return schoolDb.Roles.Where(x => x.IsActive == true).Select(r => new DropDownViewModel() { Id = r.Id, Name = r.Name }).ToList();
        }
    }
}
