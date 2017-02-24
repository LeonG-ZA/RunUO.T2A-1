using System;
using Server.Items;

namespace Server.Mobiles
{
    public class Paragon
	{
		public static double ChestChance = .10;                // Chance that a paragon will carry a paragon chest
		public static double ChocolateIngredientChance = .20;  // Chance that a paragon will drop a chocolatiering ingredient
		public static Map[] Maps = new Map[]                   // Maps that paragons will spawn on
		{
			Map.Ilshenar
		};

		private static TimeSpan FastRegenRate = TimeSpan.FromSeconds( .5 );
		private static TimeSpan CPUSaverRate = TimeSpan.FromSeconds( 2 );

		private class ParagonStamRegen : Timer
		{
			private BaseCreature m_Owner;

			public ParagonStamRegen( Mobile m )
				: base( FastRegenRate, FastRegenRate )
			{
				this.Priority = TimerPriority.FiftyMS;

				m_Owner = m as BaseCreature;
			}

			protected override void OnTick()
			{
				if( !m_Owner.Deleted && m_Owner.IsParagon && m_Owner .Map != Map.Internal )
				{
					m_Owner.Stam++;

					Delay = Interval = m_Owner.Stam < m_Owner.StamMax * .75 ? FastRegenRate : CPUSaverRate;
				}
				else
				{
					Stop();
				}
			}
		}

		public static Type[] Artifacts = new Type[]
		{
			typeof( GoldBricks ), typeof( PhillipsWoodenSteed ),
			typeof( AlchemistsBauble ), typeof( ArcticDeathDealer ),
			typeof( BlazeOfDeath ), typeof( BowOfTheJukaKing ),
			typeof( BurglarsBandana ), typeof( CavortingClub ),
			typeof( EnchantedTitanLegBone ), typeof( GwennosHarp ),
			typeof( IolosLute ), typeof( LunaLance ),
			typeof( NightsKiss ), typeof( NoxRangersHeavyCrossbow ),
			typeof( OrcishVisage ), typeof( PolarBearMask ),
			typeof( ShieldOfInvulnerability ), typeof( StaffOfPower ),
			typeof( VioletCourage ), typeof( HeartOfTheLion ),
			typeof( WrathOfTheDryad ), typeof( PixieSwatter ),
			typeof( GlovesOfThePugilist )
		};

		public static int    Hue   = 0x501;        // Paragon hue

		// Buffs
		public static double HitsBuff   = 5.0;
		public static double StrBuff    = 1.05;
		public static double IntBuff    = 1.20;
		public static double DexBuff    = 1.20;
		public static double SkillsBuff = 1.20;
		public static double SpeedBuff  = 1.20;
		public static double FameBuff   = 1.40;
		public static double KarmaBuff  = 1.40;
		public static int    DamageBuff = 5;

		public static void Convert( BaseCreature bc )
		{
			if ( bc.IsParagon )
				return;

			bc.Hue = Hue;

			if ( bc.HitsMaxSeed >= 0 )
				bc.HitsMaxSeed = (int)( bc.HitsMaxSeed * HitsBuff );

			bc.RawStr = (int)( bc.RawStr * StrBuff );
			bc.RawInt = (int)( bc.RawInt * IntBuff );
			bc.RawDex = (int)( bc.RawDex * DexBuff );

			bc.Hits = bc.HitsMax;
			bc.Mana = bc.ManaMax;
			bc.Stam = bc.StamMax;

			for( int i = 0; i < bc.Skills.Length; i++ )
			{
				Skill skill = (Skill)bc.Skills[i];

				if ( skill.Base > 0.0 )
					skill.Base *= SkillsBuff;
			}

			bc.PassiveSpeed /= SpeedBuff;
			bc.ActiveSpeed /= SpeedBuff;
			bc.CurrentSpeed = bc.PassiveSpeed;

			bc.DamageMin += DamageBuff;
			bc.DamageMax += DamageBuff;

			if ( bc.Fame > 0 )
				bc.Fame = (int)( bc.Fame * FameBuff );

			if ( bc.Fame > 32000 )
				bc.Fame = 32000;

			// TODO: Mana regeneration rate = Sqrt( buffedFame ) / 4

			if ( bc.Karma != 0 )
			{
				bc.Karma = (int)( bc.Karma * KarmaBuff );

				if( Math.Abs( bc.Karma ) > 32000 )
					bc.Karma = 32000 * Math.Sign( bc.Karma );
			}

			new ParagonStamRegen( bc ).Start();
		}

		public static void UnConvert( BaseCreature bc )
		{
			if( !bc.IsParagon )
				return;

			bc.Hue = 0;

			if( bc.HitsMaxSeed >= 0 )
				bc.HitsMaxSeed = (int)( bc.HitsMaxSeed / HitsBuff );

			bc.RawStr = (int)( bc.RawStr / StrBuff );
			bc.RawInt = (int)( bc.RawInt / IntBuff );
			bc.RawDex = (int)( bc.RawDex / DexBuff );

			bc.Hits = bc.HitsMax;
			bc.Mana = bc.ManaMax;
			bc.Stam = bc.StamMax;

			for( int i = 0; i < bc.Skills.Length; i++ )
			{
				Skill skill = (Skill)bc.Skills[i];

				if ( skill.Base > 0.0 )
					skill.Base /= SkillsBuff;
			}

			bc.PassiveSpeed *= SpeedBuff;
			bc.ActiveSpeed *= SpeedBuff;
			bc.CurrentSpeed = bc.PassiveSpeed;

			bc.DamageMin -= DamageBuff;
			bc.DamageMax -= DamageBuff;

			if ( bc.Fame > 0 )
				bc.Fame = (int)( bc.Fame / FameBuff );
			if ( bc.Karma != 0 )
				bc.Karma = (int)( bc.Karma / KarmaBuff );
		}

		public static bool CheckConvert( BaseCreature bc )
		{
			return CheckConvert( bc, bc.Location, bc.Map );
		}

		public static bool CheckConvert( BaseCreature bc, Point3D location, Map m )
		{
			return false;
		}

		public static bool CheckArtifactChance( Mobile m, BaseCreature bc )
		{
			return false;
		}

		public static void GiveArtifactTo( Mobile m )
		{
			Item item = (Item)Activator.CreateInstance( Artifacts[Utility.Random(Artifacts.Length)] );

			if ( m.AddToBackpack( item ) )
				m.SendMessage( "As a reward for slaying the mighty paragon, an artifact has been placed in your backpack." );
			else
				m.SendMessage( "As your backpack is full, your reward for destroying the legendary paragon has been placed at your feet." );
		}
	}
}