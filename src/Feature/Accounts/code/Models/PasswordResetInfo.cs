﻿using System.ComponentModel.DataAnnotations;
using Sitecore.Demo.Platform.Foundation.Dictionary.Repositories;

namespace Sitecore.Demo.Platform.Feature.Accounts.Models
{
  public class PasswordResetInfo
  {
    [Display(Name = nameof(EmailCaption), ResourceType = typeof(PasswordResetInfo))]
    [Required(ErrorMessageResourceName = nameof(Required), ErrorMessageResourceType = typeof(PasswordResetInfo))]
    [EmailAddress(ErrorMessageResourceName = nameof(InvalidEmailAddress), ErrorMessageResourceType = typeof(PasswordResetInfo))]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    public static string EmailCaption => DictionaryPhraseRepository.Current.Get("/Accounts/Forgot Password/Email", "E-mail");
    public static string Required => DictionaryPhraseRepository.Current.Get("/Accounts/Forgot Password/Required", "Please enter a value for {0}");
    public static string InvalidEmailAddress => DictionaryPhraseRepository.Current.Get("/Accounts/Forgot Password/Invalid Email Address", "Please enter a valid email address");
  }
}