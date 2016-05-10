﻿using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace IgnoreFiles
{
    public static class IgnoreClassificationTypes
    {
        public const string Keyword = "Ignore_keyword";
        public const string Operator = "Ignore_operator";
        public const string Path = "Ignore_path";

        [Export, Name(Keyword)]
        public static ClassificationTypeDefinition IgnoreClassificationBold { get; set; }

        [Export, Name(Operator)]
        public static ClassificationTypeDefinition IgnoreClassificationOperator { get; set; }

        [Export, Name(Path)]
        public static ClassificationTypeDefinition IgnoreClassificationPath { get; set; }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = IgnoreClassificationTypes.Keyword)]
    [Name(IgnoreClassificationTypes.Keyword)]
    [Order(After = Priority.Default)]
    [UserVisible(true)]
    internal sealed class IgnoreBoldFormatDefinition : ClassificationFormatDefinition
    {
        public IgnoreBoldFormatDefinition()
        {
            ForegroundBrush = Brushes.OrangeRed;
            IsBold = true;
            DisplayName = "Ignore Keyword";
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = IgnoreClassificationTypes.Operator)]
    [Name(IgnoreClassificationTypes.Operator)]
    [Order(After = IgnoreClassificationTypes.Path)]
    [UserVisible(true)]
    internal sealed class IgnoreOperatorFormatDefinition : ClassificationFormatDefinition
    {
        public IgnoreOperatorFormatDefinition()
        {
            IsItalic = true;
            DisplayName = "Ignore Operator";
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = IgnoreClassificationTypes.Path)]
    [Name(IgnoreClassificationTypes.Path)]
    [Order(After = Priority.Default)]
    [UserVisible(true)]
    internal sealed class IgnorePathFormatDefinition : ClassificationFormatDefinition
    {
        public IgnorePathFormatDefinition()
        {
            ForegroundBrush = Brushes.SteelBlue;
            DisplayName = "Ignore Path";
        }
    }
}
