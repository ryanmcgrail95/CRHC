﻿using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace general.rendering {
    public static class TextureUtility {
        public static Rect getUseRect(Rect drawRect, AspectType aspectType) {
            return getUseRect(drawRect, new Vector2(1, 1), aspectType);
        }

        public static Rect getUseRect(Rect drawRect, Texture2D texture, AspectType aspectType) {
            Vector2 contentSize;
            if (texture != null) { contentSize = new Vector2(texture.width, texture.height); }
            else { contentSize = new Vector2(1, 1); }

            return getUseRect(drawRect, contentSize, aspectType);
        }

        public static float getAspectRatio(Reference<Texture2D> texture) {
            return getAspectRatio(texture.getResource());
        }
        public static float getAspectRatio(Texture2D texture) {
            return (texture != null) ? 1f * texture.width / texture.height : 1;
        }
        public static float getAspectRatio(Vector2 contentSize) {
            return 1f * contentSize.x / contentSize.y;
        }

        public static Rect getUseRect(Rect drawRect, Vector2 contentSize, AspectType aspectType) {
            float x = drawRect.x, y = drawRect.y, w = drawRect.width, h = drawRect.height;
            float texAspect, regionAspect;

            texAspect = getAspectRatio(contentSize);

            if (h == 0) {
                regionAspect = 1;
            }
            else {
                regionAspect = w / h;
            }

            if (aspectType == AspectType.FIT_IN_REGION) {
                if (texAspect < regionAspect) {
                    x += w / 2;
                    w = h * texAspect;
                    x -= w / 2;
                }
                else {
                    y += h / 2;
                    h = w / texAspect;
                    y -= h / 2;
                }
            }
            else if (aspectType == AspectType.CROP_IN_REGION) {
                if (texAspect > regionAspect) {
                    x += w / 2;
                    w = h * texAspect;
                    x -= w / 2;
                }
                else {
                    y += h / 2;
                    h = w / texAspect;
                    y -= h / 2;
                }
            }
            else if (aspectType == AspectType.HEIGHT_DEPENDENT_ON_WIDTH) {
                h = w / texAspect;
            }

            return new Rect(x, y, w, h);
        }

        public static Rect drawTexture(Rect drawRect, Texture2D texture, AspectType aspectType) {
            Rect useRect = getUseRect(drawRect, texture, aspectType);

            if (texture != null) {
                GUIX.drawTexture(useRect, texture);
            }

            return (aspectType == AspectType.CROP_IN_REGION) ? drawRect : useRect;
        }

        private static IDictionary<Texture2D, Texture2D> targetTextures = new Dictionary<Texture2D, Texture2D>();

        private static int X_COUNT = 9, Y_COUNT = 5, INDEX_COUNT = X_COUNT * Y_COUNT;

        private static int angleToIndex(float angle) {
            return (int)Math.Round((angle + 90) / 180 * (INDEX_COUNT - 1));
        }

        private static float indexToAngle(int index) {
            return (1f * index) / (INDEX_COUNT - 1) * 180 - 90;
        }

        public static Rect drawTexture(Rect drawRect, Texture2D texture, AspectType aspectType, float angle) {
            angle %= 360;

            if (angle == 0) {
                return drawTexture(drawRect, texture, aspectType);
            }
            else {
                angle = 360 - angle;

                int w = texture.width,
                    h = texture.height,
                    s = (int)(Math.Sqrt(w * w + h * h)),
                    sf = Math.Max(w, h);

                Texture2D targetTexture;
                Rect useRect = getUseRect(drawRect, texture, aspectType);

                if (targetTextures.ContainsKey(texture)) {
                    targetTexture = targetTextures[texture];

                    float pad = (s - sf) / 2f;
                    float xO = pad, yO = pad, f = (1f * sf) / s, xF = f / X_COUNT, yF = f / Y_COUNT;

                    if (angle > 270) {
                        angle = angle - 360;
                    }
                    else if (angle > 90) {
                        xF *= -1;
                        angle = 180 - angle;
                    }

                    if (xF < 0) {
                        xO += sf;
                    }

                    int i = angleToIndex(angle);
                    int xamt, yamt;
                    xamt = i % X_COUNT;
                    yamt = (i - xamt)/X_COUNT;

                    xO += xamt * s;
                    yO += yamt * s;

                    float x0, y0;
                    x0 = xO / (s * X_COUNT);
                    y0 = yO / (s * Y_COUNT);
                   
                    Rect coords;
                    coords = new Rect(x0, y0, xF, yF);

                    GUIX.drawTexture(useRect, targetTexture, coords);
                }
                else {
                    RenderTexture rotateTexture = new RenderTexture(s * X_COUNT, s * Y_COUNT, 0);
                    targetTextures[texture] = targetTexture = new Texture2D(s * X_COUNT, s * Y_COUNT);

                    RenderTexture.active = rotateTexture;

                    GL.Clear(true, true, CrhcConstants.COLOR_TRANSPARENT);

                    GUIX.undoAllActions();

                    GL.PushMatrix();
                    GL.LoadOrtho();
                    GL.LoadPixelMatrix(0, s * X_COUNT, s * Y_COUNT, 0);

                    for (int y = 0; y < Y_COUNT; y++) {
                        for (int x = 0; x < X_COUNT; x++) {
                            Rect rect = new Rect(new Vector2(-sf / 2, -sf / 2), new Vector2(sf, sf));

                            float ang = indexToAngle(y*X_COUNT + x);

                            GL.PushMatrix();
                            GL.MultMatrix(Matrix4x4.TRS(new Vector3(s * x + s / 2, s * y + s / 2, 0), Quaternion.Euler(0, 0, ang), Vector3.one));
                            Graphics.DrawTexture(rect, texture);
                            GL.PopMatrix();
                        }
                    }

                    GL.PopMatrix();

                    targetTexture.ReadPixels(new Rect(0, 0, s * X_COUNT, s * Y_COUNT), 0, 0);
                    targetTexture.Apply();
                    GUIX.redoAllActions();

                    RenderTexture.active = null;

                    rotateTexture.Release();
                    GameObject.Destroy(rotateTexture);
                }

                return useRect;
            }
        }

        public static Rect drawTexture(Rect drawRect, Texture2D texture, Color color, AspectType aspectType) {
            GUIX.beginColor(color);
            Rect useRect = drawTexture(drawRect, texture, aspectType);
            GUIX.endColor();

            return useRect;
        }

        public static Rect drawTexture(Rect drawRect, Reference<Texture2D> textureReference, AspectType aspectType) {
            return drawTexture(drawRect, textureReference, aspectType, 0);
        }

        public static Rect drawTexture(Rect drawRect, Reference<Texture2D> textureReference, AspectType aspectType, float angle) {
            if (textureReference.isLoaded()) {
                return drawTexture(drawRect, textureReference.getResource(), aspectType, angle);
            }
            else {
                Rect useRect = getUseRect(drawRect, textureReference.getResource(), aspectType);

                GUIX.beginOpacity(.5f);
                GUIX.strokeRect(useRect, 1);
                GUIX.fillRect(new Rect(useRect.x, useRect.y, useRect.width * textureReference.getLoadFraction(), useRect.height));
                GUIX.endOpacity();

                return useRect;
            }
        }

        public static Rect drawTexture(Rect drawRect, Reference<Texture2D> textureReference, Color color, AspectType aspectType) {
            return drawTexture(drawRect, textureReference, color, aspectType, 0);
        }

        public static Rect drawTexture(Rect drawRect, Reference<Texture2D> textureReference, Color color, AspectType aspectType, float angle) {
            GUIX.beginColor(color);
            Rect useRect = drawTexture(drawRect, textureReference, aspectType, angle);
            GUIX.endColor();

            return useRect;
        }
    }
}
