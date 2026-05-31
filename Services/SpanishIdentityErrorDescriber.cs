using Microsoft.AspNetCore.Identity;

#pragma warning disable CS8765 // Nullability of parameter doesn't match overridden member (IdentityErrorDescriber base uses inconsistent annotations across SDK versions)

namespace GameVault.Services;

public class SpanishIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError DuplicateEmail(string? email) =>
        new() { Code = nameof(DuplicateEmail), Description = $"El correo {email} ya está registrado." };

    public override IdentityError DuplicateUserName(string? userName) =>
        new() { Code = nameof(DuplicateUserName), Description = $"El usuario {userName} ya está en uso." };

    public override IdentityError PasswordTooShort(int length) =>
        new() { Code = nameof(PasswordTooShort), Description = $"La contraseña debe tener al menos {length} caracteres." };

    public override IdentityError PasswordRequiresDigit() =>
        new() { Code = nameof(PasswordRequiresDigit), Description = "La contraseña debe incluir al menos un número." };

    public override IdentityError PasswordRequiresUpper() =>
        new() { Code = nameof(PasswordRequiresUpper), Description = "La contraseña debe incluir al menos una mayúscula." };

    public override IdentityError PasswordRequiresLower() =>
        new() { Code = nameof(PasswordRequiresLower), Description = "La contraseña debe incluir al menos una minúscula." };

    public override IdentityError PasswordRequiresNonAlphanumeric() =>
        new() { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "La contraseña debe incluir al menos un carácter especial." };

    public override IdentityError PasswordRequiresUniqueChars(int uniqueChars) =>
        new() { Code = nameof(PasswordRequiresUniqueChars), Description = $"La contraseña debe contener al menos {uniqueChars} caracteres distintos." };

    public override IdentityError InvalidEmail(string email) =>
        new() { Code = nameof(InvalidEmail), Description = "El correo electrónico no es válido." };

    public override IdentityError InvalidUserName(string userName) =>
        new() { Code = nameof(InvalidUserName), Description = $"El nombre de usuario '{userName}' no es válido." };

    public override IdentityError PasswordMismatch() =>
        new() { Code = nameof(PasswordMismatch), Description = "La contraseña actual es incorrecta." };

    public override IdentityError InvalidToken() =>
        new() { Code = nameof(InvalidToken), Description = "Token inválido." };

    public override IdentityError ConcurrencyFailure() =>
        new() { Code = nameof(ConcurrencyFailure), Description = "Error de concurrencia, el objeto fue modificado." };

    public override IdentityError DefaultError() =>
        new() { Code = nameof(DefaultError), Description = "Ocurrió un error desconocido." };
}
