using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameSpace.Data
{
    [System.Serializable]
    public class StimulusItem
    {
        public string type;
        public string id;
        public string text;
        public string prompt;
        public bool? hideText;
        public ImageData image;
        public AudioData audio;
        public PdfData pdf;
        public VideoData video;
        public string audioLocal;
        public string academic;
        public string showMode;
        public string subtitles;
        public int rowIndex;
        public int colIndex;
        public bool lockedPosition;
        public string expected;
        public string speaker;
        public TableData table;
        public int startRow;
        public int endRow;
        public int startCol;
        public int endCol;
        public CellData[] cells;
    }
    [System.Serializable]
    public class ImageData
    {
        public string id;
        public string url;
        public int size;
        public string sha1;
        public string mimeType;
        public int width;
        public int height;
        public string language;
        public string title;
        public int duration;
        public ThumbnailData[] thumbnails;
    }

    [System.Serializable]
    public class AudioData
    {
        public string id;
        public string url;
        public int size;
        public string sha1;
        public string mimeType;
        public int width;
        public int height;
        public string language;
        public string title;
        public int duration;
        public ThumbnailData[] thumbnails;
    }

    [System.Serializable]
    public class PdfData
    {
        // 根据实际数据结构定义
    }

    [System.Serializable]
    public class VideoData
    {
        // 根据实际数据结构定义
    }

    [System.Serializable]
    public class ThumbnailData
    {
        // 根据实际数据结构定义
    }

    [System.Serializable]
    public class TableData
    {
        // 根据实际数据结构定义
    }

    [System.Serializable]
    public class CellData
    {
        // 根据实际数据结构定义
    }

}