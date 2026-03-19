using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Schedura.Domain.Entities;
using Schedura.Domain.Enums;
using Schedura.Infra.Data;
using Schedura.Services.Users;

namespace Schedura.Bootstraper.Seeders;

public static class AdminSeeder {
	private static readonly string[] SystemActions = [
		"users.create",
		"users.read",
		"users.update",
		"users.delete",
		"usergroups.manage",
		"permissions.manage",
	];

	public static async Task SeedAsync(IServiceProvider services) {
		using var scope = services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		var adminExists = await context.Users.AnyAsync(u => u.Username == "admin");
		if (adminExists) return;

		// Create permissions
		var permissions = new List<Permission>();
		foreach (var action in SystemActions) {
			var existing = await context.Permissions.FirstOrDefaultAsync(p => p.Action == action);
			if (existing is not null) {
				permissions.Add(existing);
				continue;
			}

			var permission = new Permission(action, action, $"Permissão do sistema: {action}");
			await context.Permissions.AddAsync(permission);
			permissions.Add(permission);
		}

		await context.SaveChangesAsync();

		// Create admin group
		var adminGroup = await context.UserGroups.FirstOrDefaultAsync(g => g.Name == "Administradores");
		if (adminGroup is null) {
			adminGroup = new UserGroup("Administradores", "Grupo com acesso total ao sistema");
			await context.UserGroups.AddAsync(adminGroup);
			await context.SaveChangesAsync();
		}

		// Link permissions to group
		foreach (var permission in permissions) {
			var linkExists = await context.UserGroupPermissions
				.AnyAsync(ugp => ugp.UserGroupId == adminGroup.Id && ugp.PermissionId == permission.Id);
			if (!linkExists) {
				await context.UserGroupPermissions.AddAsync(new UserGroupPermission(adminGroup.Id, permission.Id));
			}
		}

		await context.SaveChangesAsync();

		// Create admin person
		var adminPerson = new Person(
			"Admin",
			"Schedura",
			"admin@schedura.internal",
			"00000000000",
			"Sistema",
			new DateOnly(2000, 1, 1),
			"Outro",
			"00000000000",
			PersonType.Individual);
		await context.Persons.AddAsync(adminPerson);
		await context.SaveChangesAsync();

		// Create admin user
		var hashedPassword = PasswordHasher.Hash("152369");
		var adminUser = new User("admin", hashedPassword, adminPerson.Id);
		await context.Users.AddAsync(adminUser);
		await context.SaveChangesAsync();

		// Link admin to admin group
		await context.UserUserGroups.AddAsync(new UserUserGroup(adminUser.Id, adminGroup.Id));
		await context.SaveChangesAsync();
	}
}
