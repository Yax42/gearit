using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gearit.src.robot;

namespace GeneticAlgorithm.src.Genome
{
	class SoftwareGenome : LowLevelGenome
	{
		private int m_CycleId;
		private ISpot m_Spot;

		public SoftwareGenome(RawDna rawDna, int beginning, int cycleId, ISpot spot)
			: base(rawDna, beginning)
		{
			m_CycleId = cycleId;
			m_Spot = spot;
		}

		public static void Affect(RawDna rawDna, int beginning, int cycleId, ISpot spot)
		{
			var cur = new SoftwareGenome(rawDna, beginning, cycleId, spot);
			cur.run();
		}

		private void run() // so far, costs 21 DNA molecules
		{
			int sequencesNumber = NextByte + 1;
			if (sequencesNumber > RawDna.MaxSequences)
				sequencesNumber = RawDna.MaxSequences;

			float[] motor = new float[RawDna.MaxSequences];
			Byte[] sequenceSize = new Byte[RawDna.MaxSequences];
			int totalSize = 0;

			for (int i = 0; i < sequencesNumber; i++)
			{
				motor[i] = NextRange1;
				motor[i] = motor[i] * motor[i] * motor[i]; //conserve le sign + réduit les chances d'avoir un motor très fort.
				sequenceSize[i] = NextByte;
				totalSize += sequenceSize[i];
			}

			m_RawDna.Script += "if Robot.LastTrigger = " + m_CycleId + " then\n";
			for (int i = 0; i < sequencesNumber; i++)
			{
				if (i != 0)
					m_RawDna.Script += "\telseif ";
				else
					m_RawDna.Script += "\tif ";
				m_RawDna.Script += "FrameCount % " + totalSize + " < " + sequenceSize[i] + " then\n";
				m_RawDna.Script += "\t\t" + m_Spot.Name + ".Motor = " + motor[i] + "\n";
			}
			m_RawDna.Script += "\tend\n";
			m_RawDna.Script += "end\n";
		}
	}
}
