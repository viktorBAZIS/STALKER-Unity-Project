# 🎮 STALKER Unity Project

Техническая реализация систем из вселенной S.T.A.L.K.E.R на Unity.

## 🚀 Current Systems
- **World Streaming 4.0** - Dynamic loading of 32 game locations
- **Inventory System** - Weight-based with UI events and stacking
- **Arms System** - 125+ hand models with CSV configuration
- **Project Structure Exporter** - Automated documentation generator

## 📁 Project Structure
- `Assets/Scripts/` - Game systems code
- `Assets/Models/` - 3D models (via Git LFS)
- `Assets/Scenes/` - Game locations (via Git LFS)
- `ProjectSettings/` - Unity configuration

## ⚙️ Setup Instructions
1. Clone: `git clone https://github.com/viktorBAZIS/STALKER-Unity-Project.git`
2. Install LFS: `git lfs install`
3. Pull LFS: `git lfs pull`
4. Open in Unity 2022.3+

## 🔧 Git LFS Usage
Large files stored in LFS:
- `*.fbx` - 3D models
- `*.unity` - Scene files  
- `*.png`, `*.jpg` - Textures

Repository size: ~385 MB (code) + ~2.6 GB (LFS)

## 📊 Commit History
- `56d6974` - Exclude backup scenes and heavy files
- `a5bacd9` - Initial project with LFS
- `cf33dcb` - Git LFS configuration