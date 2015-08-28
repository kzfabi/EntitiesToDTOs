//-------------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by EntitiesToDTOs.v3.1 (entitiestodtos.codeplex.com).
//     Timestamp: 2013/07/09 - 01:42:08
//
//     Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//-------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Linq;
using VS2012TestEdmx.Banking;
using System.Data.Objects.DataClasses;

namespace VS2012TestEdmx.DTOs
{

    /// <summary>
    /// Assembler for <see cref="Client"/> and <see cref="ClientDTO"/>.
    /// </summary>
    public static partial class ClientAssembler
    {
        /// <summary>
        /// Invoked when <see cref="ToDTO"/> operation is about to return.
        /// </summary>
        /// <param name="dto"><see cref="ClientDTO"/> converted from <see cref="Client"/>.</param>
        static partial void OnDTO(this Client entity, ClientDTO dto);

        /// <summary>
        /// Invoked when <see cref="ToEntity"/> operation is about to return.
        /// </summary>
        /// <param name="entity"><see cref="Client"/> converted from <see cref="ClientDTO"/>.</param>
        static partial void OnEntity(this ClientDTO dto, Client entity);

        /// <summary>
        /// Converts this instance of <see cref="ClientDTO"/> to an instance of <see cref="Client"/>.
        /// </summary>
        /// <param name="dto"><see cref="ClientDTO"/> to convert.</param>
        public static Client ToEntity(this ClientDTO dto)
        {
            if (dto == null) return null;

            var entity = new Client();

            entity.ClientID = dto.ClientID;
            entity.FirstName = dto.FirstName;
            entity.LastName = dto.LastName;
            entity.TypeOfClient = dto.TypeOfClient;

            dto.OnEntity(entity);

            return entity;
        }

        /// <summary>
        /// Converts this instance of <see cref="Client"/> to an instance of <see cref="ClientDTO"/>.
        /// </summary>
        /// <param name="entity"><see cref="Client"/> to convert.</param>
        public static ClientDTO ToDTO(this Client entity)
        {
            if (entity == null) return null;

            var dto = new ClientDTO();

            dto.ClientID = entity.ClientID;
            dto.FirstName = entity.FirstName;
            dto.LastName = entity.LastName;
            dto.TypeOfClient = entity.TypeOfClient;

            entity.OnDTO(dto);

            return dto;
        }

        /// <summary>
        /// Converts each instance of <see cref="ClientDTO"/> to an instance of <see cref="Client"/>.
        /// </summary>
        /// <param name="dtos"></param>
        /// <returns></returns>
        public static List<Client> ToEntities(this IEnumerable<ClientDTO> dtos)
        {
            if (dtos == null) return null;

            return dtos.Select(e => e.ToEntity()).ToList();
        }

        /// <summary>
        /// Converts each instance of <see cref="Client"/> to an instance of <see cref="ClientDTO"/>.
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static List<ClientDTO> ToDTOs(this IEnumerable<Client> entities)
        {
            if (entities == null) return null;

            return entities.Select(e => e.ToDTO()).ToList();
        }

    }

    /// <summary>
    /// Assembler for <see cref="Account"/> and <see cref="AccountDTO"/>.
    /// </summary>
    public static partial class AccountAssembler
    {
        /// <summary>
        /// Invoked when <see cref="ToDTO"/> operation is about to return.
        /// </summary>
        /// <param name="dto"><see cref="AccountDTO"/> converted from <see cref="Account"/>.</param>
        static partial void OnDTO(this Account entity, AccountDTO dto);

        /// <summary>
        /// Invoked when <see cref="ToEntity"/> operation is about to return.
        /// </summary>
        /// <param name="entity"><see cref="Account"/> converted from <see cref="AccountDTO"/>.</param>
        static partial void OnEntity(this AccountDTO dto, Account entity);

        /// <summary>
        /// Converts this instance of <see cref="AccountDTO"/> to an instance of <see cref="Account"/>.
        /// </summary>
        /// <param name="dto"><see cref="AccountDTO"/> to convert.</param>
        public static Account ToEntity(this AccountDTO dto)
        {
            if (dto == null) return null;

            var entity = new Account();

            entity.AccountID = dto.AccountID;
            entity.Balance = dto.Balance;
            entity.Currency = dto.Currency;
            entity.TypeOfAccount = dto.TypeOfAccount;

            dto.OnEntity(entity);

            return entity;
        }

        /// <summary>
        /// Converts this instance of <see cref="Account"/> to an instance of <see cref="AccountDTO"/>.
        /// </summary>
        /// <param name="entity"><see cref="Account"/> to convert.</param>
        public static AccountDTO ToDTO(this Account entity)
        {
            if (entity == null) return null;

            var dto = new AccountDTO();

            dto.AccountID = entity.AccountID;
            dto.Balance = entity.Balance;
            dto.Currency = entity.Currency;
            dto.TypeOfAccount = entity.TypeOfAccount;

            entity.OnDTO(dto);

            return dto;
        }

        /// <summary>
        /// Converts each instance of <see cref="AccountDTO"/> to an instance of <see cref="Account"/>.
        /// </summary>
        /// <param name="dtos"></param>
        /// <returns></returns>
        public static List<Account> ToEntities(this IEnumerable<AccountDTO> dtos)
        {
            if (dtos == null) return null;

            return dtos.Select(e => e.ToEntity()).ToList();
        }

        /// <summary>
        /// Converts each instance of <see cref="Account"/> to an instance of <see cref="AccountDTO"/>.
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static List<AccountDTO> ToDTOs(this IEnumerable<Account> entities)
        {
            if (entities == null) return null;

            return entities.Select(e => e.ToDTO()).ToList();
        }

    }
}
