#!/bin/bash
xcodebuild OTHER_CFLAGS="-fembed-bitcode" -sdk iphoneos
xcodebuild OTHER_CFLAGS="-fembed-bitcode" -sdk iphonesimulator
lipo -create -output KKSpeechRecognizer.a build/Release-iphoneos/libSpeechRecognizer.a build/Release-iphonesimulator/libSpeechRecognizer.a
