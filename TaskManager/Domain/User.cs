﻿namespace TaskManager.Domain
{
	public class User
	{
		public Guid Id { get; set; }

		public string Email { get; set; }

		public string Password { get; set; }
		
		public bool EmailConfirm { get; set; }
	}
}