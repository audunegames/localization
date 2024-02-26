using System;
using System.Collections.Generic;
using System.Linq;

namespace Audune.Localization
{
  using PluralFormatType = MessageComponent.PluralFormat.Type;


  // Class that formats a message
  internal sealed class MessageFormatter : IMessageFormatter, MessageComponent.IVisitor<string, MessageEnvironment>
  {
    // Messsage formatter properties
    public readonly IMessageFormatProvider formatProvider;
    public readonly IPluralizer pluralizer;
    public readonly IPluralizer ordinalPluralizer;


    // Constructor
    public MessageFormatter(IMessageFormatProvider formatProvider, IPluralizer pluralizer, IPluralizer ordinalPluralizer)
    {
      this.formatProvider = formatProvider;
      this.pluralizer = pluralizer;
      this.ordinalPluralizer = ordinalPluralizer;
    }


    // Format a message with the specified arguments
    public string Format(string message, IDictionary<string, object> arguments)
    {
      return Format(new Message(message), new MessageEnvironment().WithArguments(arguments));
    }

    // Format a message with the specified environment
    private string Format(Message message, MessageEnvironment env)
    {
      return string.Join("", message._components.Select(component => component.Accept(this, env)));
    }


    #region Visitor implementation
    // Visit a text component
    string MessageComponent.IVisitor<string, MessageEnvironment>.VisitTextComponent(MessageComponent.Text component, MessageEnvironment env)
    {
      var text = component.text;
      if (env.TryGetNumber(out var number))
        text = text.Replace("#", formatProvider.FormatNumber(number.value));
      return text;
    }

    // Visit a format component
    string MessageComponent.IVisitor<string, MessageEnvironment>.VisitFormatComponent(MessageComponent.Format component, MessageEnvironment env)
    {
      if (!env.TryGetArgument(component.name, out var value))
        throw new MessageException($"Argument \"{component.name}\" is not defined");

      return value switch {
        int intValue => formatProvider.FormatNumber(intValue),
        float floatValue => formatProvider.FormatNumber(floatValue),
        DateTime dateTimeValue => formatProvider.FormatDate(dateTimeValue),
        _ => value.ToString(),
      };
    }

    // Visit a number format component
    string MessageComponent.IVisitor<string, MessageEnvironment>.VisitNumberFormatComponent(MessageComponent.NumberFormat component, MessageEnvironment env)
    {
      if (!env.TryGetArgument(component.name, out var value))
        throw new MessageException($"Argument \"{component.name}\" is not defined");

      return value switch {
        int intValue => formatProvider.FormatNumber(intValue, component.style),
        float floatValue => formatProvider.FormatNumber(floatValue, component.style),
        _ => throw new MessageException($"Argument \"{component.name}\" with type {value.GetType()} is unsupported by the number format component"),
      };
    }

    // Visit a date format component
    string MessageComponent.IVisitor<string, MessageEnvironment>.VisitDateFormatComponent(MessageComponent.DateFormat component, MessageEnvironment env)
    {
      if (!env.TryGetArgument(component.name, out var value))
        throw new MessageException($"Argument \"{component.name}\" is not defined");

      return value switch {
        DateTime dateTimeValue when component.type == DateFormatType.Date => formatProvider.FormatDate(dateTimeValue, component.style),
        DateTime dateTimeValue when component.type == DateFormatType.Time => formatProvider.FormatTime(dateTimeValue, component.style),
        _ => throw new MessageException($"Argument \"{component.name}\" with type {value.GetType()} is unsupported by the date format component"),
      };
    }

    // Visit a plural format component
    string MessageComponent.IVisitor<string, MessageEnvironment>.VisitPluralFormatComponent(MessageComponent.PluralFormat component, MessageEnvironment env)
    {
      if (!env.TryGetArgument(component.name, out var value))
        throw new MessageException($"Argument \"{component.name}\" is not defined");

      var number = value switch {
        int intValue => NumberContext.Of(intValue),
        float floatValue => NumberContext.Of(floatValue),
        string stringValue => NumberContext.Of(stringValue),
        _ => throw new MessageException($"Argument \"{component.name}\" has an invalid number format"),
      };

      var keyword = component.type switch {
        PluralFormatType.Plural => pluralizer.Pluralize(number),
        PluralFormatType.Ordinal => ordinalPluralizer.Pluralize(number),
        _ => throw new MessageException($"Argument \"{component.name}\" has an invalid plural format type"),
      };

      if (component.TryGetBranch(number, keyword, out var message))
        return Format(message, env.WithNumber(number.Offset(component.offset)));
      else
        throw new MessageException($"Argument \"{component.name}\" is missing a default \"other\" keyword");
    }
    
    // Visit a select format component
    string MessageComponent.IVisitor<string, MessageEnvironment>.VisitSelectFormatComponent(MessageComponent.SelectFormat component, MessageEnvironment env)
    {
      if (!env.TryGetArgument(component.name, out var value))
        throw new MessageException($"Argument \"{component.name}\" is not defined");

      var stringValue = Convert.ToString(value);

      if (component.TryGetBranch(stringValue, out var message))
        return Format(message, env.WithoutNumber());
      else if (component.TryGetBranch("other", out var defaultMessage))
        return Format(defaultMessage, env.WithoutNumber());
      else
        throw new MessageException($"Argument \"{component.name}\" is missing a default \"other\" keyword");
    }

    // Visit a function component
    string MessageComponent.IVisitor<string, MessageEnvironment>.VisitFunctionComponent(MessageComponent.Function component, MessageEnvironment env)
    {
      return $"<Function {component.name} with argument {component.argument ?? "Null"}>";
    }

    // Visit a localization key component
    string MessageComponent.IVisitor<string, MessageEnvironment>.VisitLocalizationKeyComponent(MessageComponent.LocalizationKey component, MessageEnvironment env)
    {
      return $"<LocalizationKey \"{component.key}\">";
    }
    #endregion
  }
}