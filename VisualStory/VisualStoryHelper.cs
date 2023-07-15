using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;
using Debug = COA_DEBUG.Debug;

public static class VisualStoryHelper
{
    #region Enum
    public enum Emoticon
    {
        SWEAT,
        DOTS,
        WAA,
        SHY,
        ANNOY,
        OTL,
        E_MARK,
        Q_MARK,
        ANGRY_1,
        ANGRY_2,
        TWINKLE,
        SING
    }

    public enum PlayState
    {
        PLAYING,
        CHECKPOINT,
        BRANCH
    }

    public enum StoryPlayMode
    {
        MANUAL,
        AUTO_x1,
        AUTO_x2,
    }

    public enum OracleCategory
    {
        EVENT,
        FOLLOW,
        SUGGESTION
    }

    public enum StoryProcessButtonState
    {
        NONE,
        RESUME,
        SKIP
    }
    #endregion

    #region Variables
    public const float MIN_ZOOM_SCALE = 0.5f;
    public const float MAX_ZOOM_SCALE = 2f;
    public const float DEFAULT_ZOOM_SCALE = 1f;
    public const float BG_ZOOM_MULT = 0.1f;

    public const int USING_CHARACTER_MAX_COUNT = 5;

    public const int UNIT_OF_STORY_SPEED = 1;

    /// <summary>
    /// If current viewer count rate is equal or less than (this value)percentage of static viewer count,<para/>
    /// surely increase
    /// </summary>
    public const float VIEWER_COUNT_MIN_RATE_LIMIT = 0.8f;
    /// <summary>
    /// If current viewer count rate is equal or more than (this value)percentage of static viewer count,<para/>
    /// surely decrease
    /// </summary>
    public const float VIEWER_COUNT_MAX_RATE_LIMIT = 1.2f;
    public const float VIEWER_COUNT_CHANGE_MIN_RATE = 0.02f;
    public const float VIEWER_COUNT_CHANGE_MAX_RATE = 0.05f;
    /// <summary>
    /// sec
    /// </summary>
    public const float VIEWER_COUNT_CHANGE_FIRST_CYCLE = 3f;
    /// <summary>
    /// sec
    /// </summary>
    public const float VIEWER_COUNT_CHANGE_CYCLE = 15f;
    /// <summary>
    /// sec
    /// </summary>
    public const float VIEWER_COUNT_CHANGE_TERM = 3f;

    public static StoryProcessButtonState storyProcessButtonState;
    public static bool pressedDialogueSkipButton = false;
    #endregion

    #region Class
    public class StorySpeedData : IEnumerable
    {
        private readonly int[] arr_Speed = new int[]
        {
            UNIT_OF_STORY_SPEED,
            UNIT_OF_STORY_SPEED * 2,
            UNIT_OF_STORY_SPEED * 5,
            UNIT_OF_STORY_SPEED * 10,
        };

        public int this[int index]
        {
            get
            {
                var prev = index;
                index = Mathf.Clamp(index, 0, arr_Speed.Length - 1);
                if (prev != index) Debug.Log($"<color=red>The input index value is not available. Change to {index}");

                return arr_Speed[index];
            }
        }

        public int Length => arr_Speed.Length;

        public IEnumerator GetEnumerator()
        {
            return arr_Speed.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    public static readonly StorySpeedData _StorySpeedArrayData = new StorySpeedData();

    public class MarkerData
    {
        public IMarker marker;
        public double time => marker.time;
    }

    public class OracleChatCellData
    {
        public enum Type
        {
            NORMAL,
            DONATION,
            SPACER,
        }

        public Type cellType;
        public bool fromUser;

        #region Normal Chat Var
        public string nickname;
        public string content;
        public Data.Enum.Oracle_Chat_Format format;
        public string optional_Str;
        #endregion

        #region Donation Var
        public donation_contentsTable donationData;
        public string targetEnumId;
        #endregion

        public string _UserNickName => fromUser
            ? User.PlayerNickName
            : cellType == Type.NORMAL
            ? nickname
            : Localizer.GetLocalizedStringName(Localizer.SheetType.STRING, donationData._Sponser_Nickname);
        public float size = 0f;

        #region Constructor
        #region Spacer Type
        /// <summary>
        /// Create as Spacer
        /// </summary>
        public OracleChatCellData()
        {
            cellType = Type.SPACER;
        }
        #endregion

        #region Chat Type
        /// <summary>
        /// Create as Chat
        /// </summary>
        public OracleChatCellData(
            bool fromUser,
            string nickname,
            string content,
            Data.Enum.Oracle_Chat_Format format)
        {
            cellType = Type.NORMAL;
            this.fromUser = fromUser;
            this.nickname = nickname;
            this.content = content;
            this.format = format;
        }

        /// <summary>
        /// Create as Chat
        /// </summary>
        public OracleChatCellData(
            bool fromUser,
            string nickname,
            string content,
            Data.Enum.Oracle_Chat_Format format,
            string optional_Str) : this(fromUser, nickname, content, format)
        {
            this.optional_Str = optional_Str;
        }

        /// <summary>
        /// Create as Chat
        /// </summary>
        public OracleChatCellData(
            bool fromUser,
            string nickname,
            string content,
            Data.Enum.Oracle_Chat_Format format,
            float size) : this(fromUser, nickname, content, format)
        {
            this.size = size;
        }

        /// <summary>
        /// Create as Chat
        /// </summary>
        public OracleChatCellData(
            bool fromUser,
            string nickname,
            string content,
            Data.Enum.Oracle_Chat_Format format,
            string optional_Str,
            float size) : this(fromUser, nickname, content, format, size)
        {
            this.optional_Str = optional_Str;
        }
        #endregion

        #region Donation Type
        /// <summary>
        /// Create as Donation
        /// </summary>
        public OracleChatCellData(donation_contentsTable donationData, string targetEnumId)
        {
            cellType = Type.DONATION;
            this.donationData = donationData;
            this.targetEnumId = targetEnumId;
        }
        #endregion
        #endregion
    }

    public class OnCloseData
    {
        public enum State
        {
            NONE,
            WAITING,
            REWARD,
            ERROR
        }

        public State state = State.NONE;
        public List<UIParam.Common.Reward> list_Reward;
        public string errorMsg;
        public UIBase onErrorUI;
    }
    #endregion

    #region Methods
    public static int RandomizeViewerCount(this vstory_timelineTable staticData)
    {
        if (staticData == null) return 0;

        return (int)(staticData._Optional_Int * UnityEngine.Random.Range(VIEWER_COUNT_MIN_RATE_LIMIT, VIEWER_COUNT_MAX_RATE_LIMIT));
    }
    public static int GetRandomizeNextViewerCount(this vstory_timelineTable staticData, int prevViewerCount)
    {
        if (staticData == null) return 0;

        var standardCount = staticData._Optional_Int;

        bool positive =
            (prevViewerCount > VIEWER_COUNT_MIN_RATE_LIMIT * standardCount && prevViewerCount < VIEWER_COUNT_MAX_RATE_LIMIT * standardCount)
            ? UnityEngine.Random.Range(0, 2) == 1
            : prevViewerCount <= VIEWER_COUNT_MIN_RATE_LIMIT;
        var changeRate = UnityEngine.Random.Range(VIEWER_COUNT_CHANGE_MIN_RATE, VIEWER_COUNT_CHANGE_MAX_RATE);
        changeRate *= positive ? 1 : -1;
        return prevViewerCount + (int)(changeRate * standardCount);
    }
    #endregion

    #region Action
    public static Action<StoryProcessButtonState> onClickStoryProcessBtn;
    #endregion
}