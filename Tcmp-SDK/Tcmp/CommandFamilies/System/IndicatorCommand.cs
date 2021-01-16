using System;
using System.Collections.Generic;
using System.Linq;
using TapTrack.Tcmp.Communication;

namespace TapTrack.Tcmp.CommandFamilies.System
{
    /// <summary>
    /// Represents an indicator present on a <see cref="TappyReader"/>.
    /// </summary>
    public enum IndicatorOption
    {
        /// <summary>
        /// The red LED.
        /// </summary>
        RedLed,

        /// <summary>
        /// The green LED.
        /// </summary>
        GreenLed,

        /// <summary>
        /// The blue LED.
        /// </summary>
        BlueLed,

        /// <summary>
        /// The buzzer.
        /// </summary>
        Buzzer
    }

    /// <summary>
    /// Represents a command to modify the state of an indicator.
    /// See <see cref="IndicatorOption"/> for available options.
    /// </summary>
    public abstract class IndicatorCommand : SystemCommand
    {
        /// <summary>
        /// The command code for this IndicatorCommand.
        /// </summary>
        public abstract override byte CommandCode { get; }

        /// <summary>
        /// The target indicator.
        /// </summary>
        public abstract IndicatorOption Option { get; }
    }

    /// <summary>
    /// Represents a command to activate (turn on) an indicator.
    /// </summary>
    public sealed class ActivateIndicator : IndicatorCommand
    {
        /// <inheritdoc cref="IndicatorCommand.CommandCode"/>
        public override byte CommandCode { get; }

        /// <inheritdoc cref="IndicatorCommand.Option"/>
        public override IndicatorOption Option { get; }

        /// <summary>
        /// The amount of time (in milliseconds) to keep the indicator activated. 
        /// </summary>
        public UInt16 Duration { get; }

        /// <summary>
        /// Constructs an <see cref="ActivateIndicator"/> command that will activate the
        /// specified <see cref="IndicatorOption"/> for an optional amount of time.
        /// </summary>
        /// <param name="option">The <see cref="IndicatorOption"/> to activate.</param>
        /// <param name="duration">
        /// The amount of time (in milliseconds) to keep the indicator
        /// activated. A duration of `0` implies activation until a
        /// <see cref="DeactivateIndicator"/> command is sent.
        /// </param>
        public ActivateIndicator(IndicatorOption option, UInt16 duration = 0)
        {
            Option = option;
            Duration = (UInt16)duration;

            switch (option)
            {
                case IndicatorOption.RedLed:
                {
                    CommandCode = 0x0A;
                    break;
                }
                case IndicatorOption.GreenLed:
                {
                    CommandCode = 0x0D;
                    break;
                }
                case IndicatorOption.BlueLed:
                {
                    CommandCode = 0x10;
                    break;
                }
                case IndicatorOption.Buzzer:
                {
                    CommandCode = 0x13;
                    break;
                }
                default:
                    throw new NotImplementedException($"case {option} is not implemented");
            }

            if (Duration == 0) return;

            // Duration is > 0, so modify command code and add as parameter
            // TODO: Hard-coding this is not the most flexible solution
            CommandCode += 0x02;
            IEnumerable<byte> durationBytes = BitConverter.GetBytes(Duration).Reverse();
            parameters.AddRange(durationBytes);
        }
    }

    /// <summary>
    /// Represents a command to deactivate (turn off) an indicator.
    /// </summary>
    public sealed class DeactivateIndicator : IndicatorCommand
    {
        /// <inheritdoc cref="IndicatorCommand.CommandCode"/>
        public override byte CommandCode { get; }

        /// <inheritdoc cref="IndicatorCommand.Option"/>
        public override IndicatorOption Option { get; }

        /// <summary>
        /// Constructs a <see cref="DeactivateIndicator"/> command that will deactivate
        /// the specified <see cref="IndicatorOption"/>.
        /// </summary>
        /// <param name="option">The <see cref="IndicatorOption"/> to deactivate.</param>
        public DeactivateIndicator(IndicatorOption option)
        {
            Option = option;
            switch (option)
            {
                case IndicatorOption.RedLed:
                {
                    CommandCode = 0x0B;
                    break;
                }
                case IndicatorOption.GreenLed:
                {
                    CommandCode = 0x0E;
                    break;
                }
                case IndicatorOption.BlueLed:
                {
                    CommandCode = 0x11;
                    break;
                }
                case IndicatorOption.Buzzer:
                {
                    CommandCode = 0x13;
                    break;
                }
                default:
                    throw new NotImplementedException($"case {option} is not implemented");
            }
        }
    }
}
