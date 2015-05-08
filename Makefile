KSPDIR		:= ${HOME}/ksp/KSP_linux
MANAGED		:= ${KSPDIR}/KSP_Data/Managed
GAMEDATA	:= ${KSPDIR}/GameData
AFBWGAMEDATA  := ${GAMEDATA}/ksp-advanced-flybywire
XIDIR		:= ${GAMEDATA}/ksp-advanced-flybywire
PLUGINDIR	:= ${AFBWGAMEDATA}
TBGAMEDATA  := ${GAMEDATA}/000_Toolbar

TARGETS		:= ksp-advanced-flybywire.dll

AFBW_FILES := \
    AxisConfiguration.cs \
    Bitset.cs \
    Configuration.cs \
    ControllerPreset.cs \
    ControllerConfigurationWindow.cs \
    DefaultControllerPresets.cs \
	EVAController.cs \
    FlightManager.cs \
    FlightProperty.cs \
    KeyboardMouseController.cs \
    ModSettingsWindow.cs \
    PresetEditorWindow.cs \
    PresetEditorWindowNG.cs \
    SDL2.cs \
    SDLController.cs \
    Stringify.cs \
    StringMarshaller.cs \
    Utility.cs \
    WarpController.cs \
    EvaluationCurves.cs \
    AdvancedFlyByWire.cs \
    IController.cs \
    CameraController.cs \
    Properties/AssemblyInfo.cs \
	$e

DOC_FILES := \
	License.txt \
	README.md

RESGEN2		:= resgen2
GMCS		:= gmcs
GMCSFLAGS	:= -optimize -unsafe -d:LINUX
GIT			:= git
TAR			:= tar
ZIP			:= zip

all: ${TARGETS}

info:
	@echo "ksp-advanced-flybywire Build Information"
	@echo "    resgen2:    ${RESGEN2}"
	@echo "    gmcs:       ${GMCS}"
	@echo "    gmcs flags: ${GMCSFLAGS}"
	@echo "    git:        ${GIT}"
	@echo "    tar:        ${TAR}"
	@echo "    zip:        ${ZIP}"
	@echo "    KSP Data:   ${KSPDIR}"

ksp-advanced-flybywire.dll: ${AFBW_FILES}
	${GMCS} ${GMCSFLAGS} -t:library -lib:${XIDIR},${TBGAMEDATA},${MANAGED} \
		-r:Assembly-CSharp,Assembly-CSharp-firstpass,UnityEngine \
		-r:Toolbar \
		-out:$@ $^

clean:
	rm -f ${TARGETS} AssemblyInfo.cs

install: all
	mkdir -p ${PLUGINDIR}
	cp ${TARGETS} ${PLUGINDIR}

.PHONY: all clean install
