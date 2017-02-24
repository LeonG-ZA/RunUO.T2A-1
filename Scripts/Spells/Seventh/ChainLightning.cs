using System.Collections.Generic;
using Server.Targeting;

namespace Server.Spells.Seventh
{
    public class ChainLightningSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Chain Lightning", "Vas Ort Grav",
				209,
				9022,
				false,
				Reagent.BlackPearl,
				Reagent.Bloodmoss,
				Reagent.MandrakeRoot,
				Reagent.SulfurousAsh
			);

		public override SpellCircle Circle { get { return SpellCircle.Seventh; } }

		public ChainLightningSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public override bool DelayedDamage{ get{ return true; } }

		public void Target( IPoint3D p )
		{
			if ( !Caster.CanSee( p ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( SpellHelper.CheckTown( p, Caster ) && CheckSequence() )
			{
				SpellHelper.Turn( Caster, p );

				if ( p is Item )
					p = ((Item)p).GetWorldLocation();

				List<Mobile> targets = new List<Mobile>();

				Map map = Caster.Map;

				if ( map != null )
				{
					IPooledEnumerable eable = map.GetMobilesInRange( new Point3D( p ), 2 );

					foreach ( Mobile m in eable )
					{
						if ( SpellHelper.ValidIndirectTarget( Caster, m ) && Caster.CanBeHarmful( m, false ) )
						{
							targets.Add( m );
						}
					}

					eable.Free();
				}

				double damage = Utility.Random( 27, 22 );

				if ( targets.Count > 0 )
				{
					damage /= targets.Count;

					double toDeal;
					for ( int i = 0; i < targets.Count; ++i )
					{
						toDeal = damage;
						Mobile m = targets[i];

						if (  CheckResisted( m ) )
						{
							toDeal *= 0.5;

							m.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
						}
					toDeal *= GetDamageScalar( m );
					Caster.DoHarmful( m );
					SpellHelper.Damage( this, m, toDeal, 0, 0, 0, 0, 100 );

					m.BoltEffect( 0 );
					}
				}
				else
				{
					Caster.PlaySound ( 0x29 );
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private ChainLightningSpell m_Owner;

			public InternalTarget( ChainLightningSpell owner ) : base( 12, true, TargetFlags.None )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				IPoint3D p = o as IPoint3D;

				if ( p != null )
					m_Owner.Target( p );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}