﻿using System;
using System.Collections.Generic;
using LukaLukaLibrary.IO.Common;

namespace LukaLukaLibrary.Motions
{
    public class KeySet
    {
        public List<Key> Keys { get; }
        public bool IsInterpolated { get; set; }

        internal KeySetType Type { get; set; }

        internal void Read( EndianBinaryReader reader, bool isModern )
        {
            if ( Type == KeySetType.Static )
                Keys.Add( new Key { Value = reader.ReadSingle() } );

            else if ( Type != KeySetType.None )
            {
                IsInterpolated = Type == KeySetType.Interpolated;

                if ( isModern )
                    ReadModern();
                else
                    ReadClassic();

                void ReadClassic()
                {
                    ushort keyCount = reader.ReadUInt16();

                    Keys.Capacity = keyCount;
                    for ( int i = 0; i < keyCount; i++ )
                        Keys.Add( new Key { Frame = reader.ReadUInt16() } );

                    reader.Align( 4 );

                    foreach ( var key in Keys )
                    {
                        key.Value = reader.ReadSingle();
                        if ( IsInterpolated )
                            key.Interpolation = reader.ReadSingle();
                    }
                }

                void ReadModern()
                {
                    ushort keyCount = reader.ReadUInt16();
                    ushort type = reader.ReadUInt16();

                    Keys.Capacity = keyCount;
                    for ( int i = 0; i < keyCount; i++ )
                        Keys.Add( new Key() );

                    if ( IsInterpolated )
                        foreach ( var key in Keys )
                            key.Interpolation = reader.ReadSingle();

                    reader.Align( 4 );
                    foreach ( var key in Keys )
                        key.Value = type == 1
                            ? reader.ReadHalf()
                            : reader.ReadSingle();

                    reader.Align( 4 );
                    foreach ( var key in Keys )
                        key.Frame = reader.ReadUInt16();

                    reader.Align( 4 );
                }
            }
        }

        internal void Write( EndianBinaryWriter writer, bool isModern )
        {
            if ( Keys.Count == 1 )
                writer.Write( Keys[ 0 ].Value );

            else if ( Keys.Count > 1 )
            {
                if ( isModern )
                    WriteModern();
                else
                    WriteClassic();

                void WriteModern()
                {
                    writer.Write( ( ushort ) Keys.Count );
                    writer.Write( ( ushort ) 0 );

                    if ( IsInterpolated )
                        foreach ( var key in Keys )
                            writer.Write( key.Interpolation );

                    writer.WriteAlignmentPadding( 4 );
                    foreach ( var key in Keys )
                        writer.Write( key.Value );

                    writer.WriteAlignmentPadding( 4 );
                    foreach ( var key in Keys )
                        writer.Write( ( ushort ) key.Frame );

                    writer.WriteAlignmentPadding( 4 );
                }

                void WriteClassic()
                {
                    writer.Write( ( ushort ) Keys.Count );
                    foreach ( var key in Keys )
                        writer.Write( ( ushort ) key.Frame );

                    writer.WriteAlignmentPadding( 4 );
                    foreach ( var key in Keys )
                    {
                        writer.Write( key.Value );
                        if ( IsInterpolated )
                            writer.Write( key.Interpolation );
                    }
                }
            }
        }

        public void Merge( KeySet other )
        {
            Keys.AddRange( other.Keys );

            if ( other.IsInterpolated )
                IsInterpolated = true;
        }

        public void Sort()
        {
            Keys.Sort( ( x, y ) => x.Frame.CompareTo( y.Frame ) );
        }

        public float Interpolate( float frame )
        {
            if ( Keys.Count == 0 )
                return 0;
        
            if ( Keys.Count == 1 )
                return Keys[ 0 ].Value;

            Key previous = null;
            Key next = null;

            foreach ( var key in Keys )
            {
                if ( Math.Abs( key.Frame - frame ) < 0.000001 )
                    return key.Value;
            
                previous = next;
                next = key;

                if ( frame < next.Frame )
                    break;
            }
            
            if ( previous != null && next == null )
                return previous.Value;
                
            if ( previous == null && next != null )
                return next.Value;

            float factor = ( frame - Keys[ Keys.Count - 1 ].Frame ) /
                           ( next.Frame - Keys[ Keys.Count - 1 ].Frame );

            if ( IsInterpolated )
                return ( ( factor - 1.0f ) * 2.0f - 1.0f ) * ( factor * factor ) * ( previous.Value - next.Value ) +
                       ( ( factor - 1.0f ) * previous.Interpolation + factor * next.Interpolation ) *
                       ( factor - 1.0f ) * ( frame - Keys[ Keys.Count - 1 ].Frame ) + previous.Value;

            return ( factor * 2.0f - 3.0f ) * ( factor * factor ) * ( previous.Value - next.Value ) + previous.Value;
        }   

        public KeySet()
        {
            Keys = new List<Key>();
        }
    }
}