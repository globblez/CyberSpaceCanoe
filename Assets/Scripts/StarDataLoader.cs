using UnityEngine;
using System.IO;
using System.Collections.Generic;


//Referencing this youtube vid: https://www.youtube.com/watch?v=5Z_PvN6NMb0&list=PLbiuVVK9-w10
public class StarDataLoader {

    public class Star {
        //Variables used to define each star.
        public float catalog_number;

        public Vector3 position;    //Pos of the star
        public Color s_color;
        public float size;          //Will be proportional to the magnitude.


        //Used to calculate position using the Harvard data file
        //Private as they are only needed to calc and send to vec3 position variable

        private readonly double right_ascension; //How many radians around the circle the star is positioned at
        private readonly double declination;     //Radius above the celestial equator
        private readonly float ra_proper_motion;
        private readonly float dec_proper_motion;


        //Constructor
        public Star(float catalog_number, double right_ascension, double declination, byte spectral_type,
                    byte spectral_index, short magnitude, float ra_proper_motion, float dec_proper_motion)
        {
            this.catalog_number = catalog_number;

            //Save location parameters
            this.right_ascension = right_ascension;
            this.declination = declination;
            this.ra_proper_motion = ra_proper_motion;
            this.dec_proper_motion = dec_proper_motion;

            //Sets the position
            position = GetBasePosition();

            //Sets the color
            s_color = SetColor(spectral_type, spectral_index);

            //Sets the size
            size = SetSize(magnitude);
            
        }

        public Vector3 GetBasePosition(){ //Positions on Jan. 1st, 2000

            //Some math stuff to determine location on a cylinder surround the camera
            double x = System.Math.Cos(right_ascension);
            double y = System.Math.Sin(declination);
            double z = System.Math.Sin(right_ascension);

            //Pull/taper in stars to form a more spherical shape instead
            //Work out y-adjacent and use it to scale
            double y_cos = System.Math.Cos(declination);
            x *= y_cos;
            z *= y_cos;

            //Return vector w/ values as floats
            return new( (float)x, (float)y, (float)z );
        }

        private Color SetColor(byte spectral_type, byte spectral_index){
            //Colors are determined using real color codes of the stars

            Color IntColor(int r, int g, int b){
                return new Color(r / 255f, g / 255f, b / 255f);
            }

            Color[] col = new Color[8]; //8 different star colors (colors are the lowest value of each spectral type)
            col[0] = IntColor(0x5c, 0x7c, 0xff); // O1
            col[1] = IntColor(0x5d, 0x7e, 0xff); // B0.5
            col[2] = IntColor(0x79, 0x96, 0xff); // A0
            col[3] = IntColor(0xb8, 0xc5, 0xff); // F0
            col[4] = IntColor(0xff, 0xef, 0xed); // G1
            col[5] = IntColor(0xff, 0xde, 0xc0); // K0
            col[6] = IntColor(0xff, 0xa2, 0x5a); // M0
            col[7] = IntColor(0xff, 0x7d, 0x24); // M9.5

            int col_idx = -1; 

            if      (spectral_type == 'O')
                col_idx = 0;
            else if (spectral_type == 'B')
                col_idx = 1;
            else if (spectral_type == 'A')
                col_idx = 2;
            else if (spectral_type == 'F')
                col_idx = 3;
            else if (spectral_type == 'G')
                col_idx = 4;
            else if (spectral_type == 'K')
                col_idx = 5;
            else if (spectral_type == 'M')
                col_idx = 6; 

            //Make star white if spec type is unknown
            if (col_idx == -1)
                return Color.white;

            //Map second part 0 -> 0, 10 -> 100
            float percent = (spectral_index - 0x30) / 10.0f;

            //Gets a value between the two colors based on calculated percent?
            return Color.Lerp(col[col_idx], col[col_idx + 1], percent); 
        }

        private float SetSize(short magnitude) {
            //Inverse Lerp bc lower magnitude = brighter
            return 1 - Mathf.InverseLerp(-146, 796, magnitude); //magnitude range of the file is (-146, 796)
        }
    }

    public List<Star> LoadData(){
        List<Star> stars = new();

        //Open binary file
        const string filename = "BSC5";
        TextAsset textAsset = Resources.Load(filename) as TextAsset; //Loads file as a text asset

        MemoryStream stream = new(textAsset.bytes);
        BinaryReader br = new(stream);

        //Read the header (first 28 bytes in each file contain info on how to read it)
        //Read each of the fields
        int sequence_offset = br.ReadInt32();
        int start_index = br.ReadInt32();
        int num_stars = -br.ReadInt32(); //Should be negative to indicate coord are from the year 2000
        int star_number_settings = br.ReadInt32();
        int proper_motion_included = br.ReadInt32();
        int num_magnitudes = br.ReadInt32();
        int star_data_size = br.ReadInt32();

        //Go through each field one at a time
        for (int i = 0; i < num_stars; i++){
            float catalog_number = br.ReadSingle();  //Assuming this is the number of the star
            double right_ascension = br.ReadDouble(); 
            double declination = br.ReadDouble();    //Height above the celestial equator
            
            //Used to infer the color of the star
            byte spectral_type = br.ReadByte(); 
            byte spectral_index = br.ReadByte();

            short magnitude = br.ReadInt16(); //How bright the star is -- stored as 2-byte (16 bit) int

            //Determines motion of the star
            float ra_proper_motion = br.ReadSingle();
            float dec_proper_motion = br.ReadSingle();

            Star star = new(catalog_number, right_ascension, declination, spectral_type, 
                            spectral_index, magnitude, ra_proper_motion, dec_proper_motion); //Create new star object

            stars.Add(star); //Add star to list
        }

        return stars;
    }

    //end
}
