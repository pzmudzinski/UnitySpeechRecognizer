#!/bin/bash
xcodebuild -sdk iphoneos
xcodebuild -sdk iphonesimulator
lipo -create -output KKSpeechRecognizer.a build/Release-iphoneos/libSpeechRecognizer.a build/Release-iphonesimulator/libSpeechRecognizer.a
