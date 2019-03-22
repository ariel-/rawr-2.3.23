using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace Rawr
{
    public static class ItemIconsPet
    {
		private static ImageList _talentIcons = null;
		public static ImageList TalentIcons {
			get {
				if (_talentIcons == null) {
					_talentIcons = new ImageList();
					_talentIcons.ImageSize = new Size(45, 47);
					_talentIcons.ColorDepth = ColorDepth.Depth24Bit;
                    //_talentIcons.Images.Add("temp",*/
                        GetTalentIcon(CharacterClass.Hunter, "Ferocity", "invalid", "temp")//)
                            ;
				}
				return _talentIcons;
			}
		}

        public static Image GetItemIcon(string iconName, bool small)
        {
            Image returnImage = null;
            iconName = iconName.Replace(".png", "").Replace(".jpg", "").ToLower();
            if (small && TalentIcons.Images.ContainsKey(iconName)) {
                returnImage = TalentIcons.Images[iconName];
            } else if (!small && TalentIcons.Images.ContainsKey(iconName)) {
                returnImage = TalentIcons.Images[iconName];
            } else {
                string pathToIcon = null;
                WebRequestWrapper wrapper = null;
                try {
                    wrapper = new WebRequestWrapper();
                    if (!String.IsNullOrEmpty(iconName)) {

                        pathToIcon = wrapper.DownloadItemIcon(iconName);
                        //just in case the network code is in a disconnected mode. (e.g. no network traffic sent, so no network exception)
                    }

                    if (pathToIcon == null) {
                        pathToIcon = wrapper.DownloadTempImage();
                    }
                } catch (Exception) {
                    try {
                        pathToIcon = wrapper.DownloadTempImage();
                    } catch (Exception) { }
                    //Log.Write(ex.Message);
                    //Log.Write(ex.StackTrace);
                    //log.Error("Exception trying to retrieve an icon from the armory", ex);
                }
                if (!String.IsNullOrEmpty(pathToIcon)) {
                    int retry = 0;
                    do {
                        try {
                            using (Stream fileStream = File.Open(pathToIcon, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                                returnImage = Image.FromStream(fileStream);
                            }
                        } catch {
                            returnImage = null;
                            //possibly still downloading, give it a second
                            Thread.Sleep(TimeSpan.FromSeconds(1));
                            if (retry >= 3) {
                                //log.Error("Exception trying to load an icon from local", ex);
                                MessageBox.Show(
                                    "Rawr encountered an error while attempting to load a saved image. If you encounter this error multiple times, please ensure that Rawr is unzipped in a location that you have full file read/write access, such as your Desktop, or My Documents.");
                                //Log.Write(ex.Message);
                                //Log.Write(ex.StackTrace);
                                #if DEBUG
                                throw;
                                #endif
                            }
                        }
                        retry++;
                    } while (returnImage == null && retry < 5);

                    if (returnImage != null) {
                        if (small) {
                            returnImage = ScaleByPercent(returnImage, 50);
                        }
                        TalentIcons.Images.Add(iconName, returnImage);
                    }
                }
            }
            return returnImage;
        }

		public static Image GetTalentIcon(CharacterClass charClass, string talentTree, string talentName, string icon)
		{
			Image returnImage = null;
			string key = string.Format("{0}-{1}-{2}", charClass, talentTree, talentName);
            if (key != "Hunter-Ferocity-invalid"
                && TalentIcons.Images.ContainsKey(key)
                && TalentIcons.Images[key] == TalentIcons.Images["Hunter-Ferocity-invalid"])
            {
                return null;
            }
            if (key != "Hunter-Ferocity-invalid" && TalentIcons.Images.ContainsKey(key)) {
				returnImage = TalentIcons.Images[key];
			} else {
				string pathToIcon = null;
				WebRequestWrapper wrapper = new WebRequestWrapper();
				try {
					if (!string.IsNullOrEmpty(talentTree) && !string.IsNullOrEmpty(talentName)) {
						pathToIcon = wrapper.DownloadTalentIcon(charClass, talentTree, talentName, icon);
						//just in case the network code is in a disconnected mode. (e.g. no network traffic sent, so no network exception)
					}

					if (pathToIcon == null) {
                        // Try the Item method
                        pathToIcon = wrapper.DownloadItemIcon(icon);
                    }

					if (pathToIcon == null) {
						pathToIcon = wrapper.DownloadTempImage();
					}
				} catch (Exception) {
					pathToIcon = wrapper.DownloadTempImage();
					//Log.Write(ex.Message);
					//Log.Write(ex.StackTrace);
					//log.Error("Exception trying to retrieve an icon from the armory", ex);
				}
				if (!string.IsNullOrEmpty(pathToIcon)) {
					int retry = 0;
					do {
						try {
							using (Stream fileStream = File.Open(pathToIcon, FileMode.Open, FileAccess.Read, FileShare.Read)) {
								returnImage = Image.FromStream(fileStream);
							}
						} catch {
							returnImage = null;
							//possibly still downloading, give it a second
							Thread.Sleep(TimeSpan.FromSeconds(1));
							if (retry >= 3) {
								//log.Error("Exception trying to load an icon from local", ex);
								MessageBox.Show("Rawr encountered an error while attempting to load "
                                              + "a saved image. If you encounter this error multiple "
                                              + "times, please ensure that Rawr is unzipped in a "
                                              + "location that you have full file read/write access, "
                                              + "such as your Desktop, or My Documents."
                                              + "\r\n\r\n" + pathToIcon,
                                              "Image Loader");
								//Log.Write(ex.Message);
								//Log.Write(ex.StackTrace);
                                #if DEBUG
								throw;
                                #endif
							}
						}
						retry++;
					} while (returnImage == null && retry < 5);

					if (returnImage != null) {
						returnImage = Offset(returnImage, new Size(2, 2));
						TalentIcons.Images.Add(key, returnImage);
					}
				}
			}
			return returnImage;
		}

		private static Image Offset(Image img, Size offset)
		{
			Bitmap bmp = new Bitmap(img.Width + offset.Width, img.Height + offset.Height, PixelFormat.Format32bppArgb);
			Graphics g = Graphics.FromImage(bmp);
			g.DrawImageUnscaled(img, offset.Width, offset.Height);
			g.Dispose();
			return bmp;
		}

		private static Image ScaleByPercent(Image img, int Percent)
        {
            float nPercent = ((float) Percent/100);

            int sourceWidth = img.Width;
            int sourceHeight = img.Height;
            int sourceX = 0;
            int sourceY = 0;

            int destX = 0;
            int destY = 0;
            int destWidth = (int) (sourceWidth*nPercent);
            int destHeight = (int) (sourceHeight*nPercent);

            Bitmap bmPhoto = new Bitmap(destWidth, destHeight,
                                        PixelFormat.Format24bppRgb);
            if(Type.GetType("Mono.Runtime") == null){
            	// Only run this on .NET platforms. It breaks Mono 2.4+
                bmPhoto.SetResolution(img.HorizontalResolution,
                                      img.VerticalResolution);
            }

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(img,
                              new Rectangle(destX, destY, destWidth, destHeight),
                              new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                              GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }
    }
}