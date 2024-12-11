using System;
using SharpGDX.Graphics.G3D.Particles.Renderers;
using SharpGDX.Utils.Reflect;
using SharpGDX.Mathematics.Collision;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGDX.Graphics.G3D.Models;
using SharpGDX.Graphics.G3D.Models.Data;
using SharpGDX.Files;
using SharpGDX.Shims;
using SharpGDX.Assets.Loaders;
using SharpGDX.Utils;
using SharpGDX.Mathematics;
using Buffer = SharpGDX.Shims.Buffer;
using SharpGDX.Graphics.GLUtils;
using SharpGDX.Graphics.G3D.Environments;

namespace SharpGDX.Graphics.G3D.Particles;

/** This class is used by particle batches to sort the particles before rendering.
 * @author Inferno */
public abstract class ParticleSorter {
	static readonly Vector3 TMP_V1 = new Vector3();

	/** Using this class will not apply sorting */
	public class None : ParticleSorter {
		int currentCapacity = 0;
		int[] indices;

		public override void ensureCapacity (int capacity) {
			if (currentCapacity < capacity) {
				indices = new int[capacity];
				for (int i = 0; i < capacity; ++i)
					indices[i] = i;
				currentCapacity = capacity;
			}
		}

		public override int[] sort<T>(Array<T> renderData)
        {
			return indices;
		}
	}

    /** This class will sort all the particles using the distance from camera. */
    public class Distance : ParticleSorter
    {
        private float[] distances;
        private int[] particleIndices, particleOffsets;
        private int currentSize = 0;

        public override void ensureCapacity(int capacity)
        {
            if (currentSize < capacity)
            {
                distances = new float[capacity];
                particleIndices = new int[capacity];
                particleOffsets = new int[capacity];
                currentSize = capacity;
            }
        }

        public override int[] sort<T>(Array<T> renderData)
        {
            float[] val = camera.view.val;
            float cx = val[Matrix4.M20], cy = val[Matrix4.M21], cz = val[Matrix4.M22];
            int count = 0, i = 0;
            foreach (ParticleControllerRenderData data in
            renderData) {
                for (int k = 0, c = i + data.controller.particles.size;
                     i < c;
                     ++i, k += data.positionChannel.strideSize)
                {
                    distances[i] = cx * data.positionChannel.data[k + ParticleChannels.XOffset]
                                   + cy * data.positionChannel.data[k + ParticleChannels.YOffset]
                                   + cz * data.positionChannel.data[k + ParticleChannels.ZOffset];
                    particleIndices[i] = i;
                }

                count += data.controller.particles.size;
            }

            qsort(0, count - 1);

            for (i = 0; i < count; ++i)
            {
                particleOffsets[particleIndices[i]] = i;
            }

            return particleOffsets;
        }

        public void qsort(int si, int ei)
        {
            // base case
            if (si < ei)
            {
                float tmp;
                int tmpIndex, particlesPivotIndex;
                // insertion
                if (ei - si <= 8)
                {
                    for (int i = si; i <= ei; i++)
                    for (int j = i; j > si && distances[j - 1] > distances[j]; j--)
                    {
                        tmp = distances[j];
                        distances[j] = distances[j - 1];
                        distances[j - 1] = tmp;

                        // Swap indices
                        tmpIndex = particleIndices[j];
                        particleIndices[j] = particleIndices[j - 1];
                        particleIndices[j - 1] = tmpIndex;
                    }

                    return;
                }

                {
                    // Quick
                    float pivot = distances[si];
                    int i = si + 1;
                    particlesPivotIndex = particleIndices[si];

                    // partition array
                    for (int j = si + 1; j <= ei; j++)
                    {
                        if (pivot > distances[j])
                        {
                            if (j > i)
                            {
                                // Swap distances
                                tmp = distances[j];
                                distances[j] = distances[i];
                                distances[i] = tmp;

                                // Swap indices
                                tmpIndex = particleIndices[j];
                                particleIndices[j] = particleIndices[i];
                                particleIndices[i] = tmpIndex;
                            }

                            i++;
                        }
                    }

                    // put pivot in right position
                    distances[si] = distances[i - 1];
                    distances[i - 1] = pivot;
                    particleIndices[si] = particleIndices[i - 1];
                    particleIndices[i - 1] = particlesPivotIndex;

                    // call qsort on right and left sides of pivot
                    qsort(si, i - 2);
                    qsort(i, ei);
                }
            }
        }
    }

    protected Camera camera;

	/** @return an array of offsets where each particle should be put in the resulting mesh (also if more than one mesh will be
	 *         generated, this is an absolute offset considering a BIG output array). */
	public abstract int[] sort<T> (Array<T> renderData) where T: ParticleControllerRenderData;

	public void setCamera (Camera camera) {
		this.camera = camera;
	}

	/** This method is called when the batch has increased the underlying particle buffer. In this way the sorter can increase the
	 * data structures used to sort the particles (i.e increase backing array size) */
	public virtual void ensureCapacity (int capacity) {
	}
}
