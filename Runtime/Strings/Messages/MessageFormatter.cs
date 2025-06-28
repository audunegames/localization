using System;
using System.Collections.Generic;
using System.Linq;

namespace Audune.Localization
{
  using PluralFormatType = MessageComponent.PluralFormat.Type;


  /// <summary>
  /// Class that formats a message.
  /// </summary>
  internal sealed class MessageFormatter : IMessageFormatter, MessageComponent.IVisitor<string, MessageEnvironment>
  {
    // Messsage formatter properties
    private readonly IMessageFormatProvider _formatProvider;
    private readonly IMessageFunctionExecutor _functionExecutor;


    // Constructor
    public MessageFormatter(IMessageFormatProvider formatProvider, IMessageFunctionExecutor functionExecutor)
    {
      _formatProvider = formatProvider;
      _functionExecutor = functionExecutor;
    }


    // Format a message with the specified arguments
    public string Format(string message, IReadOnlyDictionary<string, object> arguments = null)
    {
      return Format(new Message(message), new MessageEnvironment().WithArguments(arguments ?? new Dictionary<string, object>()));
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
        text = text.Replace("#", _formatProvider.FormatNumber(number));
      return text;
    }

    // Visit a format component
    string MessageComponent.IVisitor<string, MessageEnvironment>.VisitFormatComponent(MessageComponent.Format component, MessageEnvironment env)
    {
      if (!env.TryGetArgument(component.name, out var value))
        throw new MessageException($"Argument \"{component.name}\" is not defined");

      return value switch {
        NumberContext numberValue => _formatProvider.FormatNumber(numberValue),
        int intValue => _formatProvider.FormatNumber(NumberContext.Of(intValue)),
        float floatValue => _formatProvider.FormatNumber(NumberContext.Of(floatValue)),
        DateTime dateTimeValue => _formatProvider.FormatDate(dateTimeValue),
        ILocalizedString localizedStringValue => localizedStringValue.Resolve(_formatProvider).Invoke(Format),
        _ => value.ToString(),
      };
    }

    // Visit a number format component
    string MessageComponent.IVisitor<string, MessageEnvironment>.VisitNumberFormatComponent(MessageComponent.NumberFormat component, MessageEnvironment env)
    {
      if (!env.TryGetArgument(component.name, out var value))
        throw new MessageException($"Argument \"{component.name}\" is not defined");

      return value switch {
        NumberContext numberValue => _formatProvider.FormatNumber(numberValue),
        int intValue => _formatProvider.FormatNumber(NumberContext.Of(intValue), component.style),
        float floatValue => _formatProvider.FormatNumber(NumberContext.Of(floatValue), component.style),
        _ => throw new MessageException($"Argument \"{component.name}\" with type {value.GetType()} is unsupported by the number format component"),
      };
    }

    // Visit a date format component
    string MessageComponent.IVisitor<string, MessageEnvironment>.VisitDateFormatComponent(MessageComponent.DateFormat component, MessageEnvironment env)
    {
      if (!env.TryGetArgument(component.name, out var value))
        throw new MessageException($"Argument \"{component.name}\" is not defined");

      return value switch {
        DateTime dateTimeValue when component.type == DateFormatType.Date => _formatProvider.FormatDate(dateTimeValue, component.style),
        DateTime dateTimeValue when component.type == DateFormatType.Time => _formatProvider.FormatTime(dateTimeValue, component.style),
        _ => throw new MessageException($"Argument \"{component.name}\" with type {value.GetType()} is unsupported by the date format component"),
      };
    }

    // Visit a plural format component
    string MessageComponent.IVisitor<string, MessageEnvironment>.VisitPluralFormatComponent(MessageComponent.PluralFormat component, MessageEnvironment env)
    {
      if (!env.TryGetArgument(component.name, out var value))
        throw new MessageException($"Argument \"{component.name}\" is not defined");

      var number = value switch {
        NumberContext numberValue => numberValue,
        int intValue => NumberContext.Of(intValue),
        float floatValue => NumberContext.Of(floatValue),
        string stringValue => NumberContext.Of(stringValue),
        _ => throw new MessageException($"Argument \"{component.name}\" has an invalid number format"),
      };

      var keyword = component.type switch {
        PluralFormatType.Plural => _formatProvider.pluralRules.Pluralize(number),
        PluralFormatType.Ordinal => _formatProvider.ordinalPluralRules.Pluralize(number),
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
      var argument = component.argument != null ? Format(component.argument, env) : null;

      if (_functionExecutor.TryExecuteFunction(component.name, argument, out var value))
        return Format(new Message(value), env);
      else
        throw new MessageException($"Function \"{component.name}\" could not be found");
    }

    // Visit a localization key component
    string MessageComponent.IVisitor<string, MessageEnvironment>.VisitLocalizationKeyComponent(MessageComponent.LocalizationKey component, MessageEnvironment env)
    {
      if (_formatProvider.localizedStringTable.TryFind(component.key, out var value))
        return Format(new Message(value), env);
      else
        throw new MessageException($"String reference \"{component.key}\" could not be found");
    }
    #endregion
  }
}