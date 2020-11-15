#!/bin/bash
xcodebuild -sdk iphoneos
xcodebuild -sdk iphonesimulator EXCLUDED_ARCHS="arm64"
lipo -create -output KKSpeechRecognizer.a build/Release-iphoneos/libSpeechRecognizer.a build/Release-iphonesimulator/libSpeechRecognizer.a
