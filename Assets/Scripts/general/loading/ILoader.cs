﻿using System.Collections;
using System.Collections.Generic;

public abstract class ILoader {
	private IDictionary<string, Reference> runTimeCache;

	public ILoader() {
		runTimeCache = new Dictionary<string, Reference>();
	}

	public Reference<T> tryCache<T>(string path) where T : class {
		if (runTimeCache.ContainsKey(path)) {
			return runTimeCache[path] as Reference<T>;
		}
		else {
			return null;
		}
	}
	public virtual void clearCache(bool hardClear) {
		foreach (KeyValuePair<string, Reference> pair in runTimeCache) {
			Reference r = pair.Value;
			int c = r.getOwnerCount();
			for (int i = 0; i < c; i++) {
				r.removeOwner();
			}
		}

		runTimeCache.Clear();
	}

	public Reference<T> getReference<T>(string path) where T : class {
		Reference<T> reference = tryCache<T>(path);

		if (reference == null) {
			reference = new Reference<T>(path);
			runTimeCache[path] = reference;
		}
		else {
			reference.addOwner();
		}

		return reference;
	}

	public Reference<T> load<T>(string path) where T : class {
		Reference<T> reference = tryCache<T>(path);

		if (reference == null) {
			reference = new Reference<T>(path);
			runTimeCache[path] = reference;

			load(reference);
		}
		else {
			reference.addOwner();
		}

		return reference;
	}

	public void load<T>(Reference<T> reference) where T : class {
		CoroutineManager.startCoroutine(loadCoroutine(reference));
	}

	public IEnumerator loadCoroutine<T>(Reference<T> reference) where T : class {
		yield return loadCoroutine<T>(reference, reference.getPath(), LoadType.LOAD);
	}
	public IEnumerator loadCoroutine<T>(Reference<T> reference, string path) where T : class {
		yield return loadCoroutine<T>(reference, path, LoadType.LOAD);
	}

	public abstract IEnumerator loadCoroutine<T>(Reference<T> reference, string path, LoadType loadType) where T : class;

	public IEnumerator reloadCoroutine<T>(Reference<T> reference, string path) where T : class {
		yield return loadCoroutine<T>(reference, reference.getPath(), LoadType.FORCE_RELOAD);
	}

	public IEnumerator reloadCoroutine<T>(Reference<T> reference) where T : class {
		yield return reloadCoroutine<T>(reference, reference.getPath());
	}

	public virtual int getReferenceCount() {
        return runTimeCache.Count;
    }
}

public enum SourceType {
    DEFAULT, WEB, CACHE, OFFLINE
}

public enum LoadType {
    LOAD, FORCE_RELOAD, BACKUP
}