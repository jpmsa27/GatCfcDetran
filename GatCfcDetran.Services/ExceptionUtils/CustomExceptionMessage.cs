using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatCfcDetran.Services.ExceptionUtils
{
    public static class CustomExceptionMessage
    {
        public static readonly string UserNotFound = "Usuário não encontrado.";
        public static readonly string TokenNotFound = "Token não encontrado.";
        public static readonly string AlreadyExists = "Já existe um usuário cadastrado com esse CPF.";
        public static readonly string CfcNotFound = "CFC não encontrado.";
        public static readonly string ErrorOnCreate = "Erro ao criar o usuário.";
    }
}
