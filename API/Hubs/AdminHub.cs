﻿using DataAccessLayer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Text.Json; // Add this line
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Connections.Features;
using Sprache;
using Services.PharmacyService;


public interface INotificationClient
{
    Task ReceiveNotification(string message);
    Task Receiver(string message);
    Task Render(List<Drug> medicinesNotInStock);
    Task AdminBroadcastMessage(string name, string message);
    Task ASking(List<string> medicineNames);


}
public class AdminHub : Hub<INotificationClient>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly BillService _billService;

    public AdminHub(ApplicationDbContext dbContext, BillService billService)
    {
        _dbContext = dbContext;
        _billService = billService;
    }

    public override async Task OnConnectedAsync()
    {
        //await Clients.All.ReceiveNotification($"Thank you for connecting: {Context.ConnectionId}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var connectionId = Context.ConnectionId;
        var user = await _dbContext.users.FirstOrDefaultAsync(u => u.ConnectionId == connectionId);
        if (user != null)
        {
            user.IsActive = false;
            user.ConnectionId = null;
            await _dbContext.SaveChangesAsync();
            await Clients.All.AdminBroadcastMessage(user.Name, "User has Asyncly disconnected.");
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task ManualDisconnect(string Id)
    {
        if (int.TryParse(Id, out int userId))
        {
            var user = await _dbContext.users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                user.IsActive = false;
                user.ConnectionId = null;
                await _dbContext.SaveChangesAsync();
                await Clients.All.AdminBroadcastMessage(user.Name, "User has manually disconnected.");
            }
        }
    }

    public string GetConnectionId()
    {
        return Context.ConnectionId;
    }

    public async Task Send(string id, string message)
    {
        if (int.TryParse(id, out int userId))
        {
            var user = await _dbContext.users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                user.IsActive = true;
                user.ConnectionId = Context.ConnectionId;
                await _dbContext.SaveChangesAsync();
                await Clients.All.AdminBroadcastMessage(user.Name, message);
            }
            else
            {
                await Clients.All.ReceiveNotification($"User with ID {id} not found.");
            }
        }
        else
        {
            await Clients.All.ReceiveNotification($"Invalid user ID {id}.");
        }
    }

    public async Task UsersCalling()
    {
        await UpdateUserList();
    }

    private async Task UpdateUserList()
    {
        var users = await _dbContext.users.ToListAsync();
        var usersJson = JsonSerializer.Serialize(users);
        await Clients.All.Receiver(usersJson);
    }

    

    public async Task ASking(List<string> medicineNames)
    {
        var medicinesNotInStock = await _billService.GetMedicinesNotInStock(medicineNames);
        //await Clients.All.Render(medicinesNotInStock); // Ensure this line is active to broadcast the data
    }
}
