using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
public class WaveSequencer : EditorWindow
{
    private const float TimelineDuration = 300f;
    private const float DefaultSequenceDuration = 10f;
    private const float BlockHeight = 20f;
    private List<Sequence> sequences = new List<Sequence>();
    private SequenceHandle draggingHandle;
    private Vector2 mouseDownPosition;
    private float scrollOffset;
    private float leftPanelWidth;

    private int waveNumber = 0;

    private static float TimelineIncrement = 1f;
    private static float TimelineSpacing = 30f;

    private const float pixelsPerSecond = 20f; // Dk what this is 

    private Sequence selectedSequence;
    private WaveSO selectedWaveSO;



    ButtonInfo[] buttonInfos = new ButtonInfo[]
{
        new ButtonInfo { color = new Color(1, 0.5f, 0.5f), name = "Chicken" , enemyId = SubWaveSO.EnemyID.chicken},
        new ButtonInfo { color = new Color(0.5f, 1, 0.5f), name = "Pig" , enemyId = SubWaveSO.EnemyID.pig},
        new ButtonInfo { color = new Color(0.5f, 0.5f, 1), name = "Sheep" , enemyId = SubWaveSO.EnemyID.sheep},
        new ButtonInfo { color = new Color(1, 0.7f, 0.5f), name = "Cow" , enemyId = SubWaveSO.EnemyID.cow},
        new ButtonInfo { color = new Color(0.5f, 0.7f, 1), name = "Wolf" , enemyId = SubWaveSO.EnemyID.wolf},
        new ButtonInfo { color = new Color(1, 0.5f, 0.7f), name = "Villiger" , enemyId = SubWaveSO.EnemyID.villiger},
        new ButtonInfo { color = new Color(1, 0.5f, 0.3f), name = "Iron_Golem" , enemyId = SubWaveSO.EnemyID.ironGolem}
};


    [MenuItem("Window/WaveSequencer")]
    public static void ShowWindow()
    {
        GetWindow<WaveSequencer>("Wave Sequencer");
    }


    private struct ButtonInfo
    {
        public Color color;
        public string name;
        public SubWaveSO.EnemyID enemyId; 
    }

    private void OnGUI()
    {
        leftPanelWidth = position.width * 0.3f;

        DrawTimeline();
        DrawSequences();
        ProcessInput();
        DrawLeftPanel();
       // DisplaySequenceValues();
    }

    private void DeleteSelectedSequence()
    {
        if (selectedSequence != null)
        {
            sequences.Remove(selectedSequence);
            selectedSequence = null;
        }
    }

    private void DrawLeftPanel()
    {
        Rect leftPanel = new Rect(0, 0, leftPanelWidth, position.height);
        EditorGUI.DrawRect(leftPanel, new Color(0.2f, 0.2f, 0.2f));


        Rect buttonsField = new Rect(10, 10, leftPanelWidth - 20, 80);
        EditorGUI.DrawRect(buttonsField, new Color(0.15f, 0.15f, 0.15f));

        GUILayout.BeginArea(buttonsField);
            DrawButtonsField();
        GUILayout.EndArea();

        DisplaySequenceValues(buttonsField);

        Rect timelineInfo = new Rect(10, position.height - 40, leftPanelWidth - 20, position.height -10);
        GUILayout.BeginArea(timelineInfo);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Increment Amount:", GUILayout.Width(110));
            TimelineIncrement = EditorGUILayout.FloatField(TimelineIncrement);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Timeline Spacing:", GUILayout.Width(110));
            TimelineSpacing = EditorGUILayout.FloatField(TimelineSpacing);
            EditorGUILayout.EndHorizontal();
        GUILayout.EndArea();


    }
    private void ExportWave()
    {
        WaveSO waveSO = ScriptableObject.CreateInstance<WaveSO>();

        foreach (Sequence seq in sequences)
        {
            SubWaveSO subWave = new SubWaveSO
            {
                startTime = seq.StartTime,
                enemyId = seq.EnemyId,
                frequency = seq.Frequency,
                endTime = seq.StartTime + seq.Duration
            };
            waveSO.subWaves.Add(subWave);
        }

        string path = Application.dataPath + "/Resources/WaveScriptableObjects/" + "Wave" + waveNumber + ".asset";

        if (!string.IsNullOrEmpty(path))
        {
            if (File.Exists(path)) File.Delete(path);
            path = FileUtil.GetProjectRelativePath(path);
            AssetDatabase.CreateAsset(waveSO, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
    private void ImportSelectedWave()
    {
        if (selectedWaveSO != null)
        {
            sequences.Clear();
            foreach (SubWaveSO subWave in selectedWaveSO.subWaves)
            {
                Sequence seq = new Sequence(
                subWave.startTime,
                subWave.endTime - subWave.startTime,
                GetColorFromEnemyId(subWave.enemyId),
                (int)subWave.enemyId,
                subWave.enemyId.ToString(),
                subWave.enemyId
                );
                seq.Frequency = subWave.frequency; 
                seq.BlockRect = new Rect(seq.StartTime * pixelsPerSecond, 0, seq.Duration * pixelsPerSecond, EditorGUIUtility.singleLineHeight);
                sequences.Add(seq);
            }
        }
    }

    private Color GetColorFromEnemyId(SubWaveSO.EnemyID enemyId)
    {
        switch (enemyId)
        {
            case SubWaveSO.EnemyID.chicken:
                return buttonInfos[0].color;
            case SubWaveSO.EnemyID.pig:
                return buttonInfos[1].color;
            case SubWaveSO.EnemyID.sheep:
                return buttonInfos[2].color;
            case SubWaveSO.EnemyID.cow:
                return buttonInfos[3].color;
            case SubWaveSO.EnemyID.wolf:
                return buttonInfos[4].color;
            case SubWaveSO.EnemyID.villiger:
                return buttonInfos[5].color;
            case SubWaveSO.EnemyID.ironGolem:
                return buttonInfos[6].color;
            default:
                return Color.white;
        }
    }



    private void DrawButtonsField()
    {
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();



        for (int i = 0; i < buttonInfos.Length; i++)
        {
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.normal.textColor = buttonInfos[i].color;
            buttonStyle.hover.textColor = buttonInfos[i].color;

            if (GUILayout.Button((buttonInfos[i].name).ToString(), buttonStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
            {
                AddSequence(buttonInfos[i]);
            }

            if ((i + 1) % 3 == 0)
            {
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
            }
        }

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

    private void DrawTimeline()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(leftPanelWidth);

        int numberOfIncrements = Mathf.FloorToInt(TimelineDuration / TimelineIncrement);
        for (int i = 0; i <= numberOfIncrements; i++)
        {
            float labelX = i * TimelineSpacing + scrollOffset + leftPanelWidth;
            Rect labelRect = new Rect(labelX, 0, 30, 20);
            GUI.Label(labelRect, (i * TimelineIncrement).ToString());

            // Draw vertical lines
            EditorGUI.DrawRect(new Rect(labelX, 20, 1, position.height), Color.gray);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawSequences()
    {
        float windowWidth = position.width - leftPanelWidth;
        float maxTimeDisplayed = windowWidth / 30;
        for (int i = 0; i < sequences.Count; i++)
        {
            Sequence sequence = sequences[i];
                sequence.Draw(i, scrollOffset, leftPanelWidth, selectedSequence == sequence);
        }
    }


    private void AddSequence(ButtonInfo button)
    {
        Sequence lastSequenceOfType = GetLastSequenceOfType(button.color);
        float startTimeOffset;
        float defaultSequenceDuration = Mathf.Round(DefaultSequenceDuration / TimelineIncrement) * TimelineIncrement;
        int laneIndex;

        if (lastSequenceOfType != null)
        {
            startTimeOffset = lastSequenceOfType.EndTime + 5f;
            laneIndex = lastSequenceOfType.LaneIndex;
        }
        else
        {
            startTimeOffset = Mathf.Round(5f / TimelineIncrement) * TimelineIncrement;
            laneIndex = GetNextAvailableLaneIndex();
        }

        Sequence newSequence = new Sequence(startTimeOffset, defaultSequenceDuration, button.color, laneIndex,button.name, button.enemyId);
        sequences.Add(newSequence);
        selectedSequence = newSequence;
    }
    private int GetNextAvailableLaneIndex()
    {
        int maxLaneIndex = -1;

        foreach (Sequence sequence in sequences)
        {
            if (sequence.LaneIndex > maxLaneIndex)
            {
                maxLaneIndex = sequence.LaneIndex;
            }
        }

        return maxLaneIndex + 1;
    }

    private Sequence GetLastSequenceOfType(Color color)
    {
        Sequence lastSequenceOfType = null;
        float maxEndTime = float.MinValue;

        foreach (Sequence sequence in sequences)
        {
            if (sequence.Color == color && sequence.EndTime > maxEndTime)
            {
                lastSequenceOfType = sequence;
                maxEndTime = sequence.EndTime;
            }
        }

        return lastSequenceOfType;
    }

    private void ProcessInput()
    {
        Event e = Event.current;


        if (e.type == EventType.MouseDown)
        {
            mouseDownPosition = e.mousePosition;

            if (draggingHandle == null)
            {
                foreach (Sequence sequence in sequences)
                {
                    draggingHandle = sequence.GetHandleAtMousePosition(e.mousePosition);
                    if (draggingHandle != null)
                    {
                        selectedSequence = sequence;
                        break;
                    }
                }
            }
            else
            {
                foreach (Sequence sequence in sequences)
                {
                    if (sequence.BlockRect.Contains(e.mousePosition))
                    {
                        selectedSequence = sequence;
                        break;
                    }
                }
            }
        }



        if (e.type == EventType.MouseDown)
        {
            mouseDownPosition = e.mousePosition;

            if (draggingHandle == null)
            {
                foreach (Sequence sequence in sequences)
                {
                    draggingHandle = sequence.GetHandleAtMousePosition(e.mousePosition);
                    if (draggingHandle != null)
                    {
                        break;
                    }
                }
            }
        }
        else if (e.type == EventType.MouseUp)
        {
            draggingHandle = null;
        }
        else if (e.type == EventType.MouseDrag)
        {
            if (draggingHandle != null)
            {
                draggingHandle.MoveHandle(e.mousePosition.x - mouseDownPosition.x);
                mouseDownPosition = e.mousePosition;
                Repaint();
            }
        }
        else if (e.type == EventType.ScrollWheel)
        {
            scrollOffset -= e.delta.y * 4;
            scrollOffset = Mathf.Clamp(scrollOffset, leftPanelWidth - TimelineDuration * 30, 0);
            Repaint();
        }
    }

    private void DisplaySequenceValues(Rect buttonsField)
    {
        float yOffset = buttonsField.height + buttonsField.position.y;
        Rect sequenceInfo = new Rect(10, yOffset + 10, leftPanelWidth - 20, yOffset + 80);
        if (selectedSequence != null)
        {
            GUILayout.BeginArea(sequenceInfo);
                EditorGUILayout.LabelField("Selected Sequence:", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"Name: {selectedSequence.Name}");
                EditorGUILayout.LabelField($"Start Time: {selectedSequence.StartTime:F2}");
                EditorGUILayout.LabelField($"End Time: {selectedSequence.EndTime:F2}");
                EditorGUILayout.LabelField($"Duration: {selectedSequence.Duration:F2}");

                EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Frequency:", GUILayout.Width(70));
                    selectedSequence.Frequency = EditorGUILayout.FloatField(selectedSequence.Frequency);
                    EditorGUILayout.LabelField("per second", GUILayout.Width(70));
                EditorGUILayout.EndHorizontal();



                    if (GUILayout.Button("Delete", GUILayout.Width(80), GUILayout.Height(30)))
                    {
                        DeleteSelectedSequence();
                    }




            GUILayout.EndArea();
        }

        float yOffset1 = sequenceInfo.height + sequenceInfo.position.y;
        Rect importExport = new Rect(10, yOffset1 + 10, leftPanelWidth - 20, yOffset1 + 300);
        GUILayout.BeginArea(importExport);
        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Import Wave", GUILayout.Width(80), GUILayout.Height(30)))
        {
            ImportSelectedWave();
        }
        selectedWaveSO = (WaveSO)EditorGUI.ObjectField(new Rect(80 + 5, 0, leftPanelWidth - 150, 30), selectedWaveSO, typeof(WaveSO), false);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Export Wave", GUILayout.Width(80), GUILayout.Height(30)))
        {
            ExportWave();
        }
        EditorGUILayout.LabelField("Wave Number: ", GUILayout.Width(120), GUILayout.Height(30));
        if (GUILayout.Button("<", GUILayout.Width(20), GUILayout.Height(30)))
        {
            if(waveNumber != 0) waveNumber--;
        }
        waveNumber = EditorGUILayout.IntField(waveNumber, GUILayout.Width(40), GUILayout.Height(30));
        if (GUILayout.Button(">", GUILayout.Width(20), GUILayout.Height(30)))
        {
            waveNumber++;
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Delete All", GUILayout.Width(80), GUILayout.Height(30)))
        {
            sequences.Clear();
            selectedSequence = null;
        }

        EditorGUILayout.EndVertical();
        GUILayout.EndArea();
    }
    private class Sequence
    {
        public float StartTime;
        public float Duration;
        public Rect BlockRect;
        public Color Color;
        public SubWaveSO.EnemyID EnemyId; 


        public string Name = "";
        public float EndTime => StartTime + Duration;
        public float Frequency { get; set; } = 1f;

        public Rect StartHandle { get; private set; }
        public Rect EndHandle { get; private set; }
        private const float HandleWidth = 10f;

        public int LaneIndex { get; private set; }



        public Sequence(float startTime, float duration, Color color, int laneIndex, string name, SubWaveSO.EnemyID enemyId)
        {
            StartTime = startTime;
            Duration = duration;
            Color = color;
            LaneIndex = laneIndex;
            BlockRect = new Rect(0, 0, Duration * 30, BlockHeight);
            Name = name;
            EnemyId = enemyId;
        }

        public void Draw(int index, float scrollOffset, float leftPanelWidth, bool isSelected)
        {
            float scaleFactor = TimelineSpacing / TimelineIncrement;
            BlockRect.width = Duration * scaleFactor;
            BlockRect.x = StartTime * scaleFactor + scrollOffset + leftPanelWidth;
            BlockRect.y = 30 + LaneIndex * (BlockHeight + 10);

            StartHandle = new Rect(BlockRect.x - HandleWidth, BlockRect.y, HandleWidth, BlockHeight);
            EndHandle = new Rect(BlockRect.xMax, BlockRect.y, HandleWidth, BlockHeight);


            Color displayColor = isSelected ? Color * 1.5f : Color;
            EditorGUI.DrawRect(BlockRect, displayColor);
            EditorGUI.DrawRect(StartHandle, new Color(0.5f, 0, 0));
            EditorGUI.DrawRect(EndHandle, new Color(0, 0.5f, 0));


            // Draw the name in the middle of the block
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.alignment = TextAnchor.MiddleCenter;
            labelStyle.normal.textColor = Color.black;
            EditorGUI.LabelField(new Rect(BlockRect.x, BlockRect.y, BlockRect.width, BlockRect.height), Name, labelStyle);
        }

        public void Move(float deltaX)
        {
            float scaleFactor = TimelineSpacing / TimelineIncrement;
            StartTime += deltaX / scaleFactor;
            StartTime = Mathf.Clamp(StartTime, 0, TimelineDuration - Duration);
        }

        public void ClampValues()
        {
            StartTime = Mathf.Clamp(StartTime, 0, TimelineDuration - Duration);
            Duration = Mathf.Clamp(Duration, 1, TimelineDuration - StartTime);
            UpdateBlockRectWidth();
        }

        private void UpdateBlockRectWidth()
        {
            BlockRect.width = Duration * 30;
        }

        public SequenceHandle GetHandleAtMousePosition(Vector2 mousePosition)
        {
            if (StartHandle.Contains(mousePosition))
            {
                return new SequenceHandle(this, SequenceHandle.HandleType.Start);
            }
            else if (EndHandle.Contains(mousePosition))
            {
                return new SequenceHandle(this, SequenceHandle.HandleType.End);
            }

            return null;
        }
    }
    private class SequenceHandle
    {
        public enum HandleType { Start, End }

        private Sequence sequence;
        public HandleType Type { get; private set; }

        public SequenceHandle(Sequence sequence, HandleType type)
        {
            this.sequence = sequence;
            Type = type;
        }

        public void MoveHandle(float deltaX)
        {
            float scaleFactor = TimelineSpacing / TimelineIncrement;
            float deltaUnits = deltaX / scaleFactor;

            if (Type == HandleType.Start)
            {
                sequence.StartTime += deltaUnits;
                sequence.Duration -= deltaUnits;
            }
            else if (Type == HandleType.End)
            {
                sequence.Duration += deltaUnits;
            }

            sequence.ClampValues();
        }
    }
}