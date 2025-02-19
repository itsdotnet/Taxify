﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp;
using System.Security.Claims;
using Taxify.Service.DTOs.Attachments;
using Taxify.Service.DTOs.Users;
using Taxify.Service.Interfaces;
using Taxify.Web.Models;

namespace Taxify.Web.Controllers;

public class UserController : Controller
{
    private readonly IUserService userService;
    private readonly IAttachmentService attachmentService;
    private readonly ILogger<HomeController> _logger;
    public UserController(IUserService userService, ILogger<HomeController> logger, IAttachmentService attachmentService)
    {
        _logger = logger;
        this.userService = userService;
        this.attachmentService = attachmentService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async ValueTask<IActionResult> Users()
    {
        var result = await this.userService.RetrieveAllAsync();
        
        return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> Update(UserModel model)
    {
        ClaimsPrincipal claimsUser = HttpContext.User;
        string userId = claimsUser.FindFirst(ClaimTypes.PrimarySid).Value;

        var user =  await this.userService.RetrieveByIdAsync(long.Parse(userId));

        var userUpdateDto = new UserUpdateDto
        {
            Id = long.Parse(userId),
            Firstname = model.FirstName,
            Lastname = model.LastName,
            Phone = model.Phone,
            Username = model.Username,
            Role = user.Role,
            Gender = user.Gender,
        };

        var result = await this.userService.ModifyAsync(userUpdateDto);

        return RedirectToAction("Profile","User",routeValues: model);
    }

    [HttpPost]
    public async Task<IActionResult> ImageUpload(UserModel model)
    {
        if(model.file is null)
        {
            TempData["Message"] = "Please upload image!";
            return RedirectToAction("Profile", "User");
        }

        ClaimsPrincipal claimsUser = HttpContext.User;
        string Phone = claimsUser.FindFirst(ClaimTypes.MobilePhone).Value;

        var user = await this.userService.RetrieveByPhoneAsync(Phone);

        var attachmentCreation = new AttachmentCreationDto
        {
            FormFile = model.file
        };

        var result = await this.userService.UploadImageAsync(user.Id, attachmentCreation);

        var userModel = new UserModel
        {
            FirstName = result.Firstname,
            LastName = result.Lastname,
            Phone = result.Phone,
            Image = result.Attachment.FilePath,
            Username = result.Username
        };

        return RedirectToAction("Profile", "User", userModel);
    }

    public async Task<IActionResult> Profile()
    {
        ClaimsPrincipal claimsUser = HttpContext.User;
        
        long userId = long.Parse(claimsUser.FindFirst(ClaimTypes.PrimarySid).Value);

        var result = await this.userService.RetrieveByIdAsync(userId);

        var userModel = new UserModel();

        if (result.Attachment is not null)
        {
            userModel = new UserModel
            {
                Username = result.Username,
                FirstName = result.Firstname,
                LastName = result.Lastname,
                Phone = result.Phone,
                Image = result.Attachment.FilePath.Replace("D:\\Projects\\Taxify\\Taxify.Web\\wwwroot\\", ""),
            };
        }
        else
        {
            userModel = new UserModel
            {
                Username = result.Username,
                FirstName = result.Firstname,
                LastName = result.Lastname,
                Phone = result.Phone,
                Image = null
            };
        }
        return View(userModel);
    }

}
