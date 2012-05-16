using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace MakerBarFamilyPicture
{
    class SevenSegmentDisplaysMultiplexedWithADecadeCounter
    {
        // TODO: Adapt this for arbitrary numbers of digits and numerals per number
        const int numberOfDigits = 8;

        static OutputPort[] segmentPorts;
        static OutputPort counterResetPort;
        static OutputPort counterClockPort;

        static int[] buffer = new int[numberOfDigits];

        static bool isSetUp = false;
        static bool hasBegun = false;

        static int[] numeral = new int[]
        {
            //ABCDEFG /dp 
            63, // 0
            6, // 1
            91, // 2
            79, // 3
            102, // 4
            109, // 5
            124, // 6
            7, // 7
            127, // 8
            103, // 9 
        };

        public void SetUpPorts()
        {
            segmentPorts = new OutputPort[8]
            {
                new OutputPort(Pins.GPIO_PIN_D0, false),
                new OutputPort(Pins.GPIO_PIN_D1, false),
                new OutputPort(Pins.GPIO_PIN_D2, false),
                new OutputPort(Pins.GPIO_PIN_D3, false),
                new OutputPort(Pins.GPIO_PIN_D4, false),
                new OutputPort(Pins.GPIO_PIN_D5, false),
                new OutputPort(Pins.GPIO_PIN_D6, false),
                new OutputPort(Pins.GPIO_PIN_D7, false)
            };

            counterResetPort = new OutputPort(Pins.GPIO_PIN_D8, true);
            counterClockPort = new OutputPort(Pins.GPIO_PIN_D9, false);

            isSetUp = true;
        }

        /// <summary>
        /// Initialize and prepare for updating
        /// </summary>
        public void Begin()
        {
            if (!isSetUp) SetUpPorts(); // TODO: Hacky, enforce this or make parameters part of begin()?

            counterResetPort.Write(false);

            hasBegun = true;
        }

        public void Display() // TODO: Make this asynchronous
        {
            if (!hasBegun) throw new Exception("You done goofed! SetUpPorts and Begin first, foo'!");

            counterResetPort.Write(false);

            for (int i = 0; i < numberOfDigits; i++)
            {
                DisplayDigit(buffer[i]);
                //Thread.Sleep(1);

                DisplayDigit(-1);

                counterClockPort.Write(true);
                counterClockPort.Write(false);
            }

            counterResetPort.Write(true);
        }

        private void DisplayDigit(int number)
        {
            bool isBitSet;

            for (int segment = 0; segment < 8; segment++)
            {
                if (number < 0 || number > 9)
                    isBitSet = false;
                else
                    //isBitSet = ((numeral[number] >> segment) % 2) == 1; // The easy way
                    isBitSet = (numeral[number] & ((int)System.Math.Pow(2, segment))) != 0; // Bitmaskin' Lucha Libre Style
                //isBitSet = ((bool[])numerals[number])[segment]; // Nah

                segmentPorts[segment].Write(isBitSet);
            }
        }

        /// <summary>
        /// Displays a digit in a given 2-digit display.
        /// </summary>
        /// <param name="whichDisplay">0-indexed</param>
        /// <param name="value"></param>
        public void SetDisplay(int whichDisplay, int value, bool showLeadingZeroes)
        {
            if (whichDisplay < 0 || whichDisplay > numberOfDigits / 2) return;

            if (value < 0)
            {
                buffer[whichDisplay * 2] = -1;
                buffer[whichDisplay * 2 + 1] = -1;
                return;
            }

            if (value > 99) value = 99;

            if (value < 10)
            {
                if (showLeadingZeroes)
                    buffer[whichDisplay * 2] = 0;
                else
                    buffer[whichDisplay * 2] = -1;
            }
            else
                buffer[whichDisplay * 2] = (int)System.Math.Floor(value / 10);

            buffer[whichDisplay * 2 + 1] = value % 10;
        }
    }
}
