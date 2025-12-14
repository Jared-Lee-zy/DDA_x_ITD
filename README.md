# Chordly
## AR Chord Learning & Ear-Training Application
### Readme & Walkthrough

---

## 1. Application Overview

**Chordly** is an **Augmented Reality (AR) music learning application** developed using **Unity**.
It allows users to learn and identify musical chords on **guitar and piano** through:

- AR-based instrument visualisation
- Interactive chord learning
- Audio playback
- Randomised quizzes with scoring and timing

Chordly is designed for beginners and early intermediate learners, focusing on **visual learning, ear training, and self-assessment**.

---

## 2. Platforms & Hardware Requirements

### Supported Platforms
- Android (AR-enabled devices)
- Unity Editor (for development and testing)

### Required Hardware
- Android smartphone with:
  - Camera
  - ARCore support
- Printed AR posters:
  - Guitar Poster
  - Piano Poster

### Software Requirements
- Unity (with AR Foundation installed)
- Firebase Authentication
- Firebase Realtime Database
- TextMeshPro package

---

## 3. How to Run the Application

1. Launch the application on an AR-capable Android device
2. Allow camera permissions when prompted
3. Point the camera at one of the printed AR posters:
   - Guitar Poster → Guitar mode
   - Piano Poster → Piano mode
4. A 3D instrument prefab will spawn in AR space
5. Two buttons will appear:
   - **Learn Chords**
   - **Start Quiz**

---

## 4. Controls & Interaction Guide

### General Controls
- Tap on-screen buttons to interact
- Touch-based UI only
- No physical controllers required

---

### Learn Chords Mode

**Purpose:**  
To visually and audibly learn chords.

**How to Use:**
1. Tap **Learn Chords**
2. Select a chord button (e.g. C, D, G, Em)
3. The chord diagram or fingering appears
4. Tap **Play Sound** to hear the chord
5. Learning progress updates as new chords are viewed:


**Notes:**
- Progress increases only once per chord
- Re-selecting the same chord does not increase progress

---

### Quiz Mode

**Purpose:**  
To test chord recognition by sight and sound.

**How to Use:**
1. Tap **Start Quiz**
2. The quiz canvas appears
3. Questions are shown one at a time
4. Immediate feedback is displayed
5. A results screen appears at the end

---

## 5. Quiz Walkthrough & Answer Guide

### Quiz Structure (6 Questions – Randomised)

> Question order is randomised every attempt

---

### Question Types

#### Q1–Q3: Multiple Choice
- A chord image is shown
- Select the correct chord name

#### Q4–Q5: Sound Matching
- Four sound preview buttons
- Listen to each sound
- Match the correct sound to the chord image

#### Q6: Open-Answer
- Type the chord name into the input field
- Answer checking is case-insensitive

---

### Scoring Rules
- Correct answer → +1 point
- Wrong answer → Retry allowed
- Final score displayed as:


### Timer
- Quiz completion time is recorded
- Best time is saved separately for:
  - Guitar quiz
  - Piano quiz

---

## 6. Audio Feedback

-  Correct answer → Positive sound effect
-  Wrong answer → Negative sound effect
-  Quiz completion → Completion sound
-  Background music plays during quiz mode

---

## 7. Cheats / Hacks / Shortcuts

- No cheats implemented
- Question order cannot be memorised due to shuffle system
- Users may retry questions until correct

---

## 8. Limitations & Known Bugs

### Limitations
- Limited chord set (5 chords per instrument)
- No song-learning mode implemented yet
- No difficulty levels
- Learning progress does not persist between sessions

### Known Issues
- UI may overlap on smaller screen sizes
- AR tracking may be unstable in low-light conditions
- Audio may overlap if preview buttons are pressed rapidly

---

## 9. References & Credits

### Models & Assets
- Guitar and Piano 3D models  
  [*(Insert asset source or Unity Asset Store link here)*]
  https://sketchfab.com/3d-models/dd-acoustic-guitar-4eeb8f55c0de4521af948a343094baa4
  https://sketchfab.com/3d-models/teclado-musical-keyboard-9e662d12cafa4944857015f4c5da64bf

### Images
- Guitar and Piano images
  https://www.sweelee.com.sg/products/cort-af510-op-w-acoustic-guitar-w-bag-open-pore
  https://www.sweelee.com.sg/products/arturia-minilab-mk3-25-slim-key-controller-black

### Audio
- freesound.org
- pixabay.com
- freesfx.co.uk
- https://ncs.io/skyhigh

### Tools & Libraries
- Unity
- AR Foundation
- TextMeshPro
- Firebase Authentication
- Firebase Realtime Database

---

## 10. Solutions / Answer Key (For Assessment Use)

### Chords Used (Guitar & Piano)
- C Major
- D Major
- G Major
- E Minor
- A Major

- Questions are randomized, thus answers cannot be properly specified.

### Open-Answer Question (Q6)
Accepted answers (case-insensitive):

- C B D (For Guitar)
- C E G (For Piano)

---

## 11. Version Information

**Chordify v1.1**

---

## 12. Developers

**Lim Xue Zhi Conan**  
**Jared Lee Zhengyu**  
Diploma in Immersive Media  
ITD DDA (2025–2026)


